using API.DAL.Models;

namespace API.DAL.Repositories.Abstraction;

public interface IShoppingListItemRepository : IRepository
{
    Task<ShoppingListItem?> GetByIdAsync(Guid id);
    Task<IEnumerable<ShoppingListItem>> GetByShoppingListIdAsync(Guid shoppingListId);
    Task<bool> UpdateAsync(Guid id, bool isChecked);
    Task<bool> DeleteAsync(Guid id);
    Task<ShoppingListItem> AddAsync(ShoppingListItem item);
}
