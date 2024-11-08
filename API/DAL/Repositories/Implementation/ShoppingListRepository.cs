using API.DAL.Models;
using API.DAL.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Repositories.Implementation;

public class ShoppingListRepository : IShoppingListRepository
{
    private readonly AppDbContext _db;

    public ShoppingListRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ShoppingList> CreateShoppingListAsync(
        ShoppingList shoppingList,
        List<ShoppingListItem> items
    )
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            await _db.ShoppingLists.AddAsync(shoppingList);
            await _db.SaveChangesAsync();

            foreach (var item in items)
            {
                item.ShoppingListId = shoppingList.Id;
            }
            await _db.ShoppingListItems.AddRangeAsync(items);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
            return shoppingList;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ShoppingList?> GetShoppingListByIdAsync(Guid id)
    {
        return await _db
            .ShoppingLists.Include(sl => sl.ShoppingListItems)
            .FirstOrDefaultAsync(sl => sl.Id == id);
    }

    public async Task<IEnumerable<ShoppingList>> GetUserShoppingListsAsync(Guid userId)
    {
        return await _db
            .ShoppingLists.Include(sl => sl.ShoppingListItems)
            .Where(sl => sl.UserId == userId)
            .OrderByDescending(sl => sl.GeneratedAt)
            .ToListAsync();
    }

    public async Task<bool> UpdateShoppingListItemAsync(Guid itemId, bool isChecked)
    {
        var item = await _db.ShoppingListItems.FindAsync(itemId);
        if (item == null)
            return false;

        item.IsChecked = isChecked;
        await _db.SaveChangesAsync();
        return true;
    }
}
