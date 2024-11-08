using API.DAL.Models;
using API.Requests.ShoppingLists;

namespace API.DAL.Repositories.Abstraction;

public interface IShoppingListRepository : IRepository
{
    Task<ShoppingList> CreateShoppingListAsync(
        ShoppingList shoppingList,
        List<ShoppingListItem> items
    );
    Task<ShoppingList?> GetShoppingListByIdAsync(Guid id);
    Task<IEnumerable<ShoppingList>> GetUserShoppingListsAsync(Guid userId);
    Task<bool> UpdateShoppingListItemAsync(Guid itemId, bool isChecked);
}
