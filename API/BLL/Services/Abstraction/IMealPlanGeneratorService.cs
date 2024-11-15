using API.Requests.MealPlans;

namespace API.BLL.Services.Abstraction;

public interface IMealPlanGeneratorService
{
    Task<MealPlanResponse> GenerateMealPlanAsync(MealPlanGenerateRequest request);
    Task<MealPlanResponse?> GetMealPlanByIdAsync(Guid mealPlanId);
    Task<IEnumerable<MealPlanResponse>> GetUserMealPlansAsync(Guid userId);
    Task<MealPlanResponse?> GetCurrentMealPlanAsync(Guid userId);
}
