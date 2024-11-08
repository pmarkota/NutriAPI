using API.BLL.Services.Abstraction;
using API.DAL.Models;
using API.DAL.Repositories.Abstraction;
using API.Requests.ShoppingLists;

namespace API.BLL.Services.Implementation;

public class ShoppingListItemService : IShoppingListItemService, IService
{
    private readonly IShoppingListItemRepository _repository;

    public ShoppingListItemService(IShoppingListItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<ShoppingListItemResponse?> GetByIdAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        return item != null ? MapToResponse(item) : null;
    }

    public async Task<IEnumerable<ShoppingListItemResponse>> GetByShoppingListIdAsync(
        Guid shoppingListId
    )
    {
        var items = await _repository.GetByShoppingListIdAsync(shoppingListId);
        return items.Select(MapToResponse);
    }

    public async Task<bool> ToggleItemAsync(Guid id, bool isChecked)
    {
        return await _repository.UpdateAsync(id, isChecked);
    }

    public async Task<bool> DeleteItemAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<ShoppingListItemResponse> AddItemAsync(AddShoppingListItemRequest request)
    {
        var item = new ShoppingListItem
        {
            ShoppingListId = request.ShoppingListId,
            IngredientName = request.IngredientName,
            Quantity = request.Quantity,
            Unit = request.Unit,
            IsChecked = false,
        };

        var addedItem = await _repository.AddAsync(item);
        return MapToResponse(addedItem);
    }

    private static ShoppingListItemResponse MapToResponse(ShoppingListItem item)
    {
        return new ShoppingListItemResponse
        {
            Id = item.Id,
            IngredientName = item.IngredientName ?? string.Empty,
            Quantity = item.Quantity,
            Unit = item.Unit,
            IsChecked = item.IsChecked ?? false,
        };
    }
}
