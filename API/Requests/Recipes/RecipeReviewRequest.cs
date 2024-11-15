using System.ComponentModel.DataAnnotations;

namespace API.Requests.Recipes;

public class RecipeReviewRequest
{
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    public string? Comment { get; set; }

    [Required]
    public Guid UserId { get; set; }
}
