using API.DAL.Models;
using API.Requests.MealPlans;

namespace API.DAL.Repositories.Abstraction;

public interface IMealPlanRepository
{
    Task<MealPlan> CreateMealPlanAsync(MealPlan mealPlan, List<MealPlanRecipe> mealPlanRecipes);
    Task<MealPlanResponse?> GetMealPlanByIdAsync(Guid mealPlanId);
    Task<IEnumerable<MealPlanResponse>> GetUserMealPlansAsync(Guid userId);
}
