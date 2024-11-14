namespace API.Requests.Recipes;

public class RecipesGet
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Ingredients { get; set; }
    public string? Instructions { get; set; }
    public long? Calories { get; set; }
    public long? Protein { get; set; }
    public long? Carbohydrates { get; set; }
    public long? Fats { get; set; }
    public string? DietaryLabels { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Category { get; set; }
    public string? Difficulty { get; set; }
    public int? PrepTime { get; set; }
    public int? CookingTime { get; set; }
    public int? TotalTime { get; set; }
}
