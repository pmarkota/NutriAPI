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
        // Validate request
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.UserId == Guid.Empty)
            throw new ArgumentException("Invalid user ID");

        // Validate duration with reasonable limits
        if (request.DurationInDays <= 0 || request.DurationInDays > 30)
        {
            throw new ArgumentException("Duration must be between 1 and 30 days");
        }

        // Get user preferences if needed
        UserDietaryPreferences? userPreferences = null;
        if (request.ConsiderUserPreferences)
        {
            userPreferences = await _userRepository.GetUserDietaryPreferencesAsync(request.UserId);
            if (userPreferences == null)
                throw new ArgumentException("User preferences not found");
        }

        // Determine the dietary preference to use
        var dietaryPreference = !string.IsNullOrEmpty(request.SpecificDietaryPreference)
            ? request.SpecificDietaryPreference
            : userPreferences?.DietaryPreference;

        if (string.IsNullOrEmpty(dietaryPreference))
            throw new ArgumentException("Dietary preference must be specified");

        var caloricGoal = request.SpecificCaloricGoal ?? userPreferences?.CaloricGoal;

        if (!caloricGoal.HasValue)
            throw new ArgumentException("Caloric goal must be specified");

        // Get suitable recipes based on preferences
        var recipes = await _recipeRepository.GetFilteredRecipesAsync(
            new RecipeFilterRequest
            {
                DietaryPreference = dietaryPreference, // Use the dietary preference as a string
                MaxCalories = caloricGoal.Value / 3 + 200, // Allow some flexibility per meal
            }
        );

        if (!recipes.Any())
        {
            throw new InvalidOperationException(
                $"No recipes found matching dietary preference: {dietaryPreference} "
                    + $"and max calories: {caloricGoal.Value / 3 + 200}"
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

    public async Task<MealPlanResponse?> GetCurrentMealPlanAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Invalid user ID");

        return await _mealPlanRepository.GetCurrentMealPlanAsync(userId);
    }

    private async Task<DailyMealPlan> GenerateDailyPlanAsync(
        List<RecipesGet> availableRecipes,
        long targetDailyCalories,
        DateOnly date
    )
    {
        var meals = new List<MealPlanRecipeInfo>();
        var dailyCalories = 0L;
        var dailyProtein = 0L;
        var dailyCarbs = 0L;
        var dailyFats = 0L;
        var mealTypes = new[] { "Breakfast", "Lunch", "Dinner", "Snack" };

        // Continue adding meals until we reach or exceed the target daily calories
        while (dailyCalories < targetDailyCalories)
        {
            foreach (var mealType in mealTypes)
            {
                if (dailyCalories >= targetDailyCalories)
                    break;

                var remainingCalories = targetDailyCalories - dailyCalories;
                var meal = SelectAppropriateRecipe(
                    availableRecipes,
                    remainingCalories,
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
        if (recipes == null || !recipes.Any())
            throw new ArgumentException("Recipe list cannot be null or empty");

        if (targetCalories <= 0)
            throw new ArgumentException("Target calories must be greater than 0");

        if (excludeRecipeIds == null)
            throw new ArgumentException("Exclude recipe IDs list cannot be null");

        // Get available recipes excluding already used ones
        var availableRecipes = recipes.Where(r => !excludeRecipeIds.Contains(r.Id)).ToList();
        if (!availableRecipes.Any())
        {
            availableRecipes = recipes.ToList(); // Reset if no unique recipes left
        }

        // Start with a small tolerance and increase it with each attempt
        var tolerance = Math.Min(100 * (attempt + 1), 300);

        // Try to find recipes within the current tolerance
        var suitableRecipes = availableRecipes
            .Where(r =>
                r.Calories.HasValue && Math.Abs(r.Calories.Value - targetCalories) <= tolerance
            )
            .ToList();

        if (suitableRecipes.Any())
        {
            // If we found suitable recipes, pick the one closest to target calories
            return suitableRecipes
                .OrderBy(r => Math.Abs((r.Calories ?? 0) - targetCalories))
                .First();
        }

        // If no suitable recipes found and we haven't reached max retries, try again with larger tolerance
        if (attempt < MaxRetries)
        {
            return SelectAppropriateRecipe(recipes, targetCalories, excludeRecipeIds, attempt + 1);
        }

        // If all else fails, pick the closest recipe regardless of tolerance
        return availableRecipes.OrderBy(r => Math.Abs((r.Calories ?? 0) - targetCalories)).First();
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
