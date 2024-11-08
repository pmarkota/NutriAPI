using API.DAL.Models;
using API.DAL.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Repositories.Implementation;

public class ShoppingListItemRepository : IShoppingListItemRepository, IRepository
{
    private readonly AppDbContext _db;

    public ShoppingListItemRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ShoppingListItem?> GetByIdAsync(Guid id)
    {
        return await _db
            .ShoppingListItems.Include(item => item.ShoppingList)
            .FirstOrDefaultAsync(item => item.Id == id);
    }

    public async Task<IEnumerable<ShoppingListItem>> GetByShoppingListIdAsync(Guid shoppingListId)
    {
        return await _db
            .ShoppingListItems.Where(item => item.ShoppingListId == shoppingListId)
            .OrderBy(item => item.IngredientName)
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(Guid id, bool isChecked)
    {
        var item = await _db.ShoppingListItems.FindAsync(id);
        if (item == null)
            return false;

        item.IsChecked = isChecked;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var item = await _db.ShoppingListItems.FindAsync(id);
        if (item == null)
            return false;

        _db.ShoppingListItems.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<ShoppingListItem> AddAsync(ShoppingListItem item)
    {
        await _db.ShoppingListItems.AddAsync(item);
        await _db.SaveChangesAsync();
        return item;
    }
}
