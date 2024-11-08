namespace API.Requests.ShoppingLists;

public class AddShoppingListItemRequest
{
    public Guid ShoppingListId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public float? Quantity { get; set; }
    public string? Unit { get; set; }
}
