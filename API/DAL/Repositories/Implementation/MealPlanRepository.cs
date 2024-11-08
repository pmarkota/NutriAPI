using API.DAL.Models;
using API.DAL.Repositories.Abstraction;
using API.Requests.MealPlans;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Repositories.Implementation;

public class MealPlanRepository : IMealPlanRepository, IRepository
{
    private readonly AppDbContext _db;

    public MealPlanRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<MealPlan> CreateMealPlanAsync(
        MealPlan mealPlan,
        List<MealPlanRecipe> mealPlanRecipes
    )
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            await _db.MealPlans.AddAsync(mealPlan);
            await _db.SaveChangesAsync();

            foreach (var recipe in mealPlanRecipes)
            {
                recipe.MealPlanId = mealPlan.Id;
            }

            await _db.MealPlanRecipes.AddRangeAsync(mealPlanRecipes);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
            return mealPlan;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<MealPlanResponse?> GetMealPlanByIdAsync(Guid mealPlanId)
    {
        var mealPlan = await _db
            .MealPlans.Include(mp => mp.MealPlanRecipes)
            .ThenInclude(mpr => mpr.Recipe)
            .FirstOrDefaultAsync(mp => mp.Id == mealPlanId);

        if (mealPlan == null)
            return null;

        return await CreateMealPlanResponseAsync(mealPlan);
    }

    public async Task<IEnumerable<MealPlanResponse>> GetUserMealPlansAsync(Guid userId)
    {
        var mealPlans = await _db
            .MealPlans.Include(mp => mp.MealPlanRecipes)
            .ThenInclude(mpr => mpr.Recipe)
            .Where(mp => mp.UserId == userId)
            .ToListAsync();

        var responses = new List<MealPlanResponse>();
        foreach (var mealPlan in mealPlans)
        {
            responses.Add(await CreateMealPlanResponseAsync(mealPlan));
        }

        return responses;
    }

    private async Task<MealPlanResponse> CreateMealPlanResponseAsync(MealPlan mealPlan)
    {
        var dailyPlans = mealPlan
            .MealPlanRecipes.GroupBy(mpr => mpr.Day)
            .Select(group => new DailyMealPlan
            {
                Day = group.Key ?? string.Empty,
                Date = GetDateFromDay(
                    mealPlan.StartDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                    group.Key ?? ""
                ),
                Meals = group
                    .Select(mpr => new MealPlanRecipeInfo
                    {
                        RecipeId = mpr.RecipeId ?? Guid.Empty,
                        RecipeName = mpr.Recipe?.Name ?? string.Empty,
                        MealType = mpr.MealType ?? string.Empty,
                        Calories = mpr.Recipe?.Calories ?? 0,
                        Protein = mpr.Recipe?.Protein,
                        Carbohydrates = mpr.Recipe?.Carbohydrates,
                        Fats = mpr.Recipe?.Fats,
                    })
                    .ToList(),
                DailyTotalCalories = group.Sum(mpr => mpr.Recipe?.Calories ?? 0),
                DailyTotalProtein = group.Sum(mpr => mpr.Recipe?.Protein ?? 0),
                DailyTotalCarbohydrates = group.Sum(mpr => mpr.Recipe?.Carbohydrates ?? 0),
                DailyTotalFats = group.Sum(mpr => mpr.Recipe?.Fats ?? 0),
            })
            .ToList();

        return new MealPlanResponse
        {
            Id = mealPlan.Id,
            PlanName = mealPlan.PlanName ?? string.Empty,
            StartDate = mealPlan.StartDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = mealPlan.EndDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            TotalCalories = mealPlan.CaloricTotal ?? 0,
            TotalProtein = dailyPlans.Sum(d => d.DailyTotalProtein),
            TotalCarbohydrates = dailyPlans.Sum(d => d.DailyTotalCarbohydrates),
            TotalFats = dailyPlans.Sum(d => d.DailyTotalFats),
            DailyPlans = dailyPlans,
        };
    }

    private DateOnly GetDateFromDay(DateOnly startDate, string day)
    {
        if (Enum.TryParse<DayOfWeek>(day, out var dayOfWeek))
        {
            int daysToAdd = ((int)dayOfWeek - (int)startDate.DayOfWeek + 7) % 7;
            return startDate.AddDays(daysToAdd);
        }
        return startDate;
    }
}
