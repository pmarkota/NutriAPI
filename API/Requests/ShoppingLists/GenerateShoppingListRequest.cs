namespace API.Requests.ShoppingLists;

public class GenerateShoppingListRequest
{
    public Guid UserId { get; set; }
    public Guid MealPlanId { get; set; }
}
