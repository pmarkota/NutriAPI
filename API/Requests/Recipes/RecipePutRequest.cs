using System.ComponentModel.DataAnnotations;

namespace API.Requests.Recipes;

public class RecipePutRequest
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public List<string>? Ingredients { get; set; }

    public string? Instructions { get; set; }

    public long? Calories { get; set; }

    public long? Protein { get; set; }

    public long? Carbohydrates { get; set; }

    public long? Fats { get; set; }

    public string? DietaryLabels { get; set; }
}
