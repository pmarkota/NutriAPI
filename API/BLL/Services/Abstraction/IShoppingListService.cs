using API.Requests.ShoppingLists;

namespace API.BLL.Services.Abstraction;

public interface IShoppingListService : IService
{
    Task<ShoppingListResponse> GenerateFromMealPlanAsync(Guid userId, Guid mealPlanId);
    Task<ShoppingListResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<ShoppingListResponse>> GetUserShoppingListsAsync(Guid userId);
    Task<bool> ToggleItemCheckAsync(Guid itemId, bool isChecked);
}
