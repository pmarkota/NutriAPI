namespace API.Requests.Recipes;

public class RecipeFilterRequest
{
    public string? DietaryPreference { get; set; }
    public long? MaxCalories { get; set; }
    public long? MinProtein { get; set; }
    public long? MaxCarbohydrates { get; set; }
    public long? MaxFats { get; set; }
}
