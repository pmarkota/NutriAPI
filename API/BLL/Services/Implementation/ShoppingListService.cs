using System.Text.Json;
using API.BLL.Services.Abstraction;
using API.DAL.Models;
using API.DAL.Repositories.Abstraction;
using API.Requests.ShoppingLists;

namespace API.BLL.Services.Implementation;

public class ShoppingListService : IShoppingListService
{
    private readonly IShoppingListRepository _shoppingListRepository;
    private readonly IMealPlanRepository _mealPlanRepository;
    private readonly IRecipeRepository _recipeRepository;

    public ShoppingListService(
        IShoppingListRepository shoppingListRepository,
        IMealPlanRepository mealPlanRepository,
        IRecipeRepository recipeRepository
    )
    {
        _shoppingListRepository = shoppingListRepository;
        _mealPlanRepository = mealPlanRepository;
        _recipeRepository = recipeRepository;
    }

    public async Task<ShoppingListResponse> GenerateFromMealPlanAsync(Guid userId, Guid mealPlanId)
    {
        var mealPlan = await _mealPlanRepository.GetMealPlanByIdAsync(mealPlanId);
        if (mealPlan == null)
            throw new ArgumentException("Meal plan not found");

        // Get the original meal plan to check user access
        var originalMealPlan = await _mealPlanRepository.GetOriginalMealPlanAsync(mealPlanId);
        if (originalMealPlan?.UserId != userId)
            throw new UnauthorizedAccessException("User does not have access to this meal plan");

        var ingredients = new Dictionary<string, (float Quantity, string Unit)>();

        // Collect all recipes from the meal plan
        foreach (var dailyPlan in mealPlan.DailyPlans)
        {
            foreach (var meal in dailyPlan.Meals)
            {
                var recipe = await _recipeRepository.GetRecipeByIdAsync(meal.RecipeId);
                if (recipe?.Ingredients == null)
                    continue;

                try
                {
                    // Parse ingredients as array of objects
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

                        if (ingredients.ContainsKey(name))
                        {
                            var (existingQuantity, existingUnit) = ingredients[name];
                            if (existingUnit == ingredient.Unit)
                            {
                                ingredients[name] = (
                                    existingQuantity + ingredient.Quantity,
                                    ingredient.Unit
                                );
                            }
                            else
                            {
                                // If units don't match, keep them separate
                                ingredients[$"{name} ({ingredient.Unit})"] = (
                                    ingredient.Quantity,
                                    ingredient.Unit
                                );
                            }
                        }
                        else
                        {
                            ingredients[name] = (ingredient.Quantity, ingredient.Unit);
                        }
                    }
                }
                catch (JsonException ex)
                {
                    // Log the error and continue with next recipe
                    Console.WriteLine(
                        $"Error parsing ingredients for recipe {recipe.Id}: {ex.Message}"
                    );
                    continue;
                }
            }
        }

        // Create shopping list
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
        public float Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}
