using API.Requests.ShoppingLists;

namespace API.BLL.Services.Abstraction;

public interface IShoppingListItemService : IService
{
    Task<ShoppingListItemResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<ShoppingListItemResponse>> GetByShoppingListIdAsync(Guid shoppingListId);
    Task<bool> ToggleItemAsync(Guid id, bool isChecked);
    Task<bool> DeleteItemAsync(Guid id);
    Task<ShoppingListItemResponse> AddItemAsync(AddShoppingListItemRequest request);
}
