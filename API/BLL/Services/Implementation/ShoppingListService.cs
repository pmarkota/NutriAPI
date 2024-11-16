using System.Text.Json;
using API.BLL.Services.Abstraction;
using API.DAL.Models;
using API.DAL.Repositories.Abstraction;
using API.Requests.ShoppingLists;
using Microsoft.Extensions.Logging;

namespace API.BLL.Services.Implementation;

public class ShoppingListService : IShoppingListService
{
    private readonly IShoppingListRepository _shoppingListRepository;
    private readonly IMealPlanRepository _mealPlanRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserPreferencesRepository _userPreferencesRepository;
    private readonly ILogger<ShoppingListService> _logger;

    public ShoppingListService(
        IShoppingListRepository shoppingListRepository,
        IMealPlanRepository mealPlanRepository,
        IRecipeRepository recipeRepository,
        IUserPreferencesRepository userPreferencesRepository,
        ILogger<ShoppingListService> logger
    )
    {
        _shoppingListRepository = shoppingListRepository;
        _mealPlanRepository = mealPlanRepository;
        _recipeRepository = recipeRepository;
        _userPreferencesRepository = userPreferencesRepository;
        _logger = logger;
    }

    public async Task<ShoppingListResponse> GenerateFromMealPlanAsync(Guid userId, Guid mealPlanId)
    {
        var mealPlan = await _mealPlanRepository.GetMealPlanByIdAsync(mealPlanId);

        if (mealPlan == null)
            throw new ArgumentException("Meal plan not found");

        var originalMealPlan = await _mealPlanRepository.GetOriginalMealPlanAsync(mealPlanId);

        if (originalMealPlan?.UserId != userId)
            throw new UnauthorizedAccessException("User does not have access to this meal plan");

        var ingredients = new Dictionary<string, (float Quantity, string Unit)>();

        foreach (var dailyPlan in mealPlan.DailyPlans)
        {
            foreach (var meal in dailyPlan.Meals)
            {
                var recipe = await _recipeRepository.GetRecipeByIdAsync(meal.RecipeId);

                if (recipe?.Ingredients == null)
                    continue;

                try
                {
                    var mealIngredients = JsonSerializer.Deserialize<List<IngredientItem>>(
                        recipe.Ingredients,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (mealIngredients == null)
                        continue;

                    foreach (var ingredient in mealIngredients)
                    {
                        if (string.IsNullOrEmpty(ingredient.Name))
                            continue;

                        var name = ingredient.Name.Trim();
                        var (quantity, unit) = ingredient.ParseQuantity();

                        if (quantity == 0)
                            continue;

                        if (ingredients.ContainsKey(name))
                        {
                            var (existingQuantity, existingUnit) = ingredients[name];

                            if (existingUnit.ToLowerInvariant() == unit.ToLowerInvariant())
                            {
                                ingredients[name] = (existingQuantity + quantity, existingUnit);
                            }
                            else
                            {
                                var newName = $"{name} ({unit})";

                                if (ingredients.ContainsKey(newName))
                                {
                                    var (qty, u) = ingredients[newName];

                                    ingredients[newName] = (qty + quantity, u);
                                }
                                else
                                {
                                    ingredients[newName] = (quantity, unit);
                                }
                            }
                        }
                        else
                        {
                            ingredients[name] = (quantity, unit);
                        }
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, $"Error parsing ingredients for recipe {recipe.Id}");
                    continue;
                }
            }
        }

        var shoppingList = new ShoppingList
        {
            UserId = userId,
            MealPlanId = mealPlanId,
            GeneratedAt = DateTime.UtcNow,
        };

        var shoppingListItems = ingredients
            .Select(kvp => new ShoppingListItem
            {
                IngredientName = kvp.Key,
                Quantity = kvp.Value.Quantity,
                Unit = kvp.Value.Unit,
                IsChecked = false,
            })
            .ToList();

        var createdList = await _shoppingListRepository.CreateShoppingListAsync(
            shoppingList,
            shoppingListItems
        );

        return MapToResponse(createdList);
    }

    public async Task<ShoppingListResponse?> GetByIdAsync(Guid id)
    {
        var shoppingList = await _shoppingListRepository.GetShoppingListByIdAsync(id);

        return shoppingList != null ? MapToResponse(shoppingList) : null;
    }

    public async Task<IEnumerable<ShoppingListResponse>> GetUserShoppingListsAsync(Guid userId)
    {
        var shoppingLists = await _shoppingListRepository.GetUserShoppingListsAsync(userId);

        return shoppingLists.Select(MapToResponse);
    }

    public async Task<bool> ToggleItemCheckAsync(Guid itemId, bool isChecked)
    {
        return await _shoppingListRepository.UpdateShoppingListItemAsync(itemId, isChecked);
    }

    public async Task<ShoppingListResponse> GenerateFromFavoritesAsync(Guid userId)
    {
        var userPreferences = await _userPreferencesRepository.GetUserPreferencesAsync(userId);
        if (userPreferences == null)
            throw new ArgumentException("User preferences not found");

        _logger.LogInformation($"Raw favorite recipes JSON: {userPreferences.FavoriteRecipes}");

        var favoriteRecipeIds = JsonSerializer.Deserialize<List<Guid>>(
            userPreferences.FavoriteRecipes,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (favoriteRecipeIds == null || !favoriteRecipeIds.Any())
            throw new InvalidOperationException("No favorite recipes found");

        _logger.LogInformation($"Found {favoriteRecipeIds.Count} favorite recipes");

        var ingredients = new Dictionary<string, (float Quantity, string Unit)>();

        foreach (var recipeId in favoriteRecipeIds)
        {
            _logger.LogInformation($"Processing recipe ID: {recipeId}");
            var recipe = await _recipeRepository.GetRecipeByIdAsync(recipeId);
            
            if (recipe?.Ingredients == null)
            {
                _logger.LogWarning($"Recipe {recipeId} not found or has no ingredients");
                continue;
            }

            _logger.LogInformation($"Recipe {recipeId} ingredients raw data: {recipe.Ingredients}");

            try
            {
                var recipeIngredients = JsonSerializer.Deserialize<List<IngredientItem>>(
                    recipe.Ingredients,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (recipeIngredients == null)
                {
                    _logger.LogWarning($"Failed to deserialize ingredients for recipe {recipeId}");
                    continue;
                }

                _logger.LogInformation($"Found {recipeIngredients.Count} ingredients in recipe {recipeId}");

                foreach (var ingredient in recipeIngredients)
                {
                    if (string.IsNullOrEmpty(ingredient.Name))
                        continue;

                    var name = ingredient.Name.Trim();
                    var (quantity, unit) = ingredient.ParseQuantity();

                    _logger.LogInformation($"Processing ingredient: {name}, Quantity: {quantity}, Unit: {unit}");

                    if (quantity == 0)
                        continue;

                    if (ingredients.ContainsKey(name))
                    {
                        var (existingQuantity, existingUnit) = ingredients[name];

                        if (existingUnit.ToLowerInvariant() == unit.ToLowerInvariant())
                        {
                            ingredients[name] = (existingQuantity + quantity, existingUnit);
                        }
                        else
                        {
                            var newName = $"{name} ({unit})";

                            if (ingredients.ContainsKey(newName))
                            {
                                var (qty, u) = ingredients[newName];

                                ingredients[newName] = (qty + quantity, u);
                            }
                            else
                            {
                                ingredients[newName] = (quantity, unit);
                            }
                        }
                    }
                    else
                    {
                        ingredients[name] = (quantity, unit);
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Error parsing ingredients for recipe {recipeId}: {ex.Message}");
                continue;
            }
        }

        var shoppingList = new ShoppingList
        {
            UserId = userId,
            GeneratedAt = DateTime.UtcNow
        };

        var shoppingListItems = ingredients
            .Select(kvp => new ShoppingListItem
            {
                IngredientName = kvp.Key,
                Quantity = kvp.Value.Quantity,
                Unit = kvp.Value.Unit,
                IsChecked = false
            })
            .ToList();

        var createdList = await _shoppingListRepository.CreateShoppingListAsync(
            shoppingList,
            shoppingListItems
        );

        return MapToResponse(createdList);
    }

    private static ShoppingListResponse MapToResponse(ShoppingList shoppingList)
    {
        return new ShoppingListResponse
        {
            Id = shoppingList.Id,
            UserId = shoppingList.UserId ?? Guid.Empty,
            MealPlanId = shoppingList.MealPlanId ?? Guid.Empty,
            GeneratedAt = shoppingList.GeneratedAt,
            Items = shoppingList
                .ShoppingListItems.Select(item => new ShoppingListItemResponse
                {
                    Id = item.Id,
                    IngredientName = item.IngredientName ?? string.Empty,
                    Quantity = item.Quantity,
                    Unit = item.Unit,
                    IsChecked = item.IsChecked ?? false,
                })
                .ToList(),
        };
    }

    private class IngredientItem
    {
        public string Name { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;

        public (float Number, string Unit) ParseQuantity()
        {
            if (string.IsNullOrEmpty(Quantity))
                return (0, string.Empty);

            var quantity = Quantity.Trim().ToLowerInvariant();

            float number = 0;
            string unit = string.Empty;

            switch (quantity)
            {
                case "one":
                case "a":
                case "an":
                case "1 clove":
                case "1":
                    return (1, "piece");
                case "two":
                    return (2, "piece");
                case "three":
                    return (3, "piece");
                case "half":
                case "1/2":
                    return (0.5f, "piece");
                case "quarter":
                case "1/4":
                    return (0.25f, "piece");
                case "to taste":
                case "as needed":
                case "pinch":
                    return (1, "to taste");
            }

            if (quantity.Contains('/'))
            {
                var parts = quantity.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 1)
                {
                    var fractionParts = parts[0].Split('/');

                    if (
                        fractionParts.Length == 2
                        && float.TryParse(fractionParts[0], out float numerator)
                        && float.TryParse(fractionParts[1], out float denominator)
                        && denominator != 0
                    )
                    {
                        number = numerator / denominator;

                        unit = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : "piece";

                        return (number, StandardizeUnit(unit));
                    }
                }
            }

            var match = System.Text.RegularExpressions.Regex.Match(
                quantity,
                @"^(\d+\.?\d*)\s*([a-zA-Z\s]+|)$"
            );

            if (match.Success)
            {
                if (float.TryParse(match.Groups[1].Value, out float num))
                {
                    number = num;

                    unit = match.Groups[2].Value.Trim();

                    return (number, StandardizeUnit(unit));
                }
            }

            return (1, "to taste");
        }
    }

    private static string StandardizeUnit(string unit)
    {
        switch (unit.Trim().ToLowerInvariant())
        {
            case "g":
            case "gram":
            case "grams":
                return "g";
            case "kg":
            case "kilogram":
            case "kilograms":
                return "kg";
            case "ml":
            case "milliliter":
            case "milliliters":
            case "millilitre":
            case "millilitres":
                return "ml";
            case "l":
            case "liter":
            case "liters":
            case "litre":
            case "litres":
                return "l";
            case "tbsp":
            case "tablespoon":
            case "tablespoons":
                return "tbsp";
            case "tsp":
            case "teaspoon":
            case "teaspoons":
                return "tsp";
            case "cup":
            case "cups":
                return "cup";
            case "oz":
            case "ounce":
            case "ounces":
                return "oz";
            case "lb":
            case "lbs":
            case "pound":
            case "pounds":
                return "lb";
            case "piece":
            case "pieces":
            case "pc":
            case "pcs":
            case "":
                return "piece";
            case "slice":
            case "slices":
                return "slice";
            case "clove":
            case "cloves":
                return "clove";
            default:
                return unit.Trim();
        }
    }
}