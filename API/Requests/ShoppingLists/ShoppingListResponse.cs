namespace API.Requests.ShoppingLists;

public class ShoppingListResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MealPlanId { get; set; }
    public DateTime GeneratedAt { get; set; }
    public List<ShoppingListItemResponse> Items { get; set; } = new();
}

public class ShoppingListItemResponse
{
    public Guid Id { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public float? Quantity { get; set; }
    public string? Unit { get; set; }
    public bool IsChecked { get; set; }
}
