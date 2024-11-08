using API.BLL.Services.Abstraction;
using API.DAL.Models;
using API.DAL.Repositories.Abstraction;
using API.Requests.MealPlans;
using API.Requests.Recipes;
using API.Requests.Users;

namespace API.BLL.Services.Implementation;

public class MealPlanGeneratorService : IMealPlanGeneratorService, IService
{
    private readonly IUserRepository _userRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IMealPlanRepository _mealPlanRepository;
    private const int MaxRetries = 5;

    public MealPlanGeneratorService(
        IUserRepository userRepository,
        IRecipeRepository recipeRepository,
        IMealPlanRepository mealPlanRepository
    )
    {
        _userRepository = userRepository;
        _recipeRepository = recipeRepository;
        _mealPlanRepository = mealPlanRepository;
    }

    public async Task<MealPlanResponse> GenerateMealPlanAsync(MealPlanGenerateRequest request)
    {
        // Validate duration
        if (request.DurationInDays <= 0)
        {
            throw new ArgumentException("Duration must be greater than 0 days");
        }

        // Get user preferences if needed
        UserDietaryPreferences? userPreferences = null;
        if (request.ConsiderUserPreferences)
        {
            userPreferences = await _userRepository.GetUserDietaryPreferencesAsync(request.UserId);
            if (userPreferences == null)
                throw new ArgumentException("User preferences not found");
        }

        // Use specified preferences or fall back to user preferences
        var dietaryPreference =
            request.SpecificDietaryPreference ?? userPreferences?.DietaryPreference;
        var caloricGoal = request.SpecificCaloricGoal ?? userPreferences?.CaloricGoal;

        if (!caloricGoal.HasValue)
            throw new ArgumentException("Caloric goal must be specified");

        // Get suitable recipes based on preferences
        var recipes = await _recipeRepository.GetFilteredRecipesAsync(
            new RecipeFilterRequest
            {
                DietaryPreference = dietaryPreference,
                MaxCalories =
                    caloricGoal.Value / 3
                    + 200 // Allow some flexibility per meal
                ,
            }
        );

        if (!recipes.Any())
        {
            throw new InvalidOperationException(
                $"No recipes found matching dietary preference: {dietaryPreference ?? "None"} "
                    + $"and max calories: {caloricGoal.Value / 3 + 200}"
            );
        }

        // Check if we have enough recipes for the plan
        var minimumRecipesNeeded = 3; // One for each meal type
        if (recipes.Count() < minimumRecipesNeeded)
        {
            throw new InvalidOperationException(
                $"Not enough recipes available. Found {recipes.Count()} recipes, "
                    + $"but need at least {minimumRecipesNeeded} for a daily plan"
            );
        }

        // Generate daily meal plans
        var dailyPlans = new List<DailyMealPlan>();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(request.DurationInDays - 1);

        for (int i = 0; i < request.DurationInDays; i++)
        {
            var currentDate = startDate.AddDays(i);
            var dailyPlan = await GenerateDailyPlanAsync(
                recipes.ToList(),
                caloricGoal.Value,
                currentDate
            );
            dailyPlans.Add(dailyPlan);
        }

        // Create and save the meal plan
        var mealPlan = new MealPlan
        {
            UserId = request.UserId,
            PlanName = $"Meal Plan {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
            StartDate = startDate,
            EndDate = endDate,
            CaloricTotal = dailyPlans.Sum(d => d.DailyTotalCalories),
            CreatedAt = DateTime.UtcNow,
        };

        var mealPlanRecipes = dailyPlans
            .SelectMany(dailyPlan =>
                dailyPlan.Meals.Select(meal => new MealPlanRecipe
                {
                    RecipeId = meal.RecipeId,
                    Day = dailyPlan.Day,
                    MealType = meal.MealType,
                    CreatedAt = DateTime.UtcNow,
                })
            )
            .ToList();

        var savedMealPlan = await _mealPlanRepository.CreateMealPlanAsync(
            mealPlan,
            mealPlanRecipes
        );

        return new MealPlanResponse
        {
            Id = savedMealPlan.Id,
            PlanName = savedMealPlan.PlanName ?? string.Empty,
            StartDate = savedMealPlan.StartDate ?? startDate,
            EndDate = savedMealPlan.EndDate ?? endDate,
            TotalCalories = savedMealPlan.CaloricTotal ?? 0,
            TotalProtein = dailyPlans.Sum(d => d.DailyTotalProtein),
            TotalCarbohydrates = dailyPlans.Sum(d => d.DailyTotalCarbohydrates),
            TotalFats = dailyPlans.Sum(d => d.DailyTotalFats),
            DailyPlans = dailyPlans,
        };
    }

    private async Task<DailyMealPlan> GenerateDailyPlanAsync(
        List<RecipesGet> availableRecipes,
        long targetCalories,
        DateOnly date
    )
    {
        var meals = new List<MealPlanRecipeInfo>();
        var dailyCalories = 0L;
        var dailyProtein = 0L;
        var dailyCarbs = 0L;
        var dailyFats = 0L;
        var mealTypes = new[] { "Breakfast", "Lunch", "Dinner" };
        var targetPerMeal = targetCalories / mealTypes.Length;

        foreach (var mealType in mealTypes)
        {
            var meal = SelectAppropriateRecipe(
                availableRecipes,
                targetPerMeal,
                meals.Select(m => m.RecipeId).ToList()
            );

            meals.Add(
                new MealPlanRecipeInfo
                {
                    RecipeId = meal.Id,
                    RecipeName = meal.Name,
                    MealType = mealType,
                    Calories = meal.Calories ?? 0,
                    Protein = meal.Protein,
                    Carbohydrates = meal.Carbohydrates,
                    Fats = meal.Fats,
                }
            );

            dailyCalories += meal.Calories ?? 0;
            dailyProtein += meal.Protein ?? 0;
            dailyCarbs += meal.Carbohydrates ?? 0;
            dailyFats += meal.Fats ?? 0;
        }

        return new DailyMealPlan
        {
            Date = date,
            Day = date.DayOfWeek.ToString(),
            Meals = meals,
            DailyTotalCalories = dailyCalories,
            DailyTotalProtein = dailyProtein,
            DailyTotalCarbohydrates = dailyCarbs,
            DailyTotalFats = dailyFats,
        };
    }

    private RecipesGet SelectAppropriateRecipe(
        List<RecipesGet> recipes,
        long targetCalories,
        List<Guid> excludeRecipeIds,
        int attempt = 0
    )
    {
        // First check if we have any recipes at all after excluding used ones
        var availableRecipes = recipes.Where(r => !excludeRecipeIds.Contains(r.Id)).ToList();
        if (!availableRecipes.Any())
        {
            throw new InvalidOperationException(
                "Not enough unique recipes available for the meal plan"
            );
        }

        if (attempt >= MaxRetries)
        {
            // If we've tried too many times, just return the recipe closest to target calories
            return availableRecipes
                .OrderBy(r => Math.Abs((r.Calories ?? 0) - targetCalories))
                .First();
        }

        var tolerance = Math.Min(200 * (attempt + 1), 500); // Increase tolerance with each retry
        var suitableRecipes = availableRecipes
            .Where(r =>
                r.Calories.HasValue && Math.Abs(r.Calories.Value - targetCalories) <= tolerance
            )
            .ToList();

        if (!suitableRecipes.Any())
        {
            return SelectAppropriateRecipe(recipes, targetCalories, excludeRecipeIds, attempt + 1);
        }

        return suitableRecipes[new Random().Next(suitableRecipes.Count)];
    }

    public async Task<MealPlanResponse?> GetMealPlanByIdAsync(Guid mealPlanId)
    {
        return await _mealPlanRepository.GetMealPlanByIdAsync(mealPlanId);
    }

    public async Task<IEnumerable<MealPlanResponse>> GetUserMealPlansAsync(Guid userId)
    {
        return await _mealPlanRepository.GetUserMealPlansAsync(userId);
    }
}
