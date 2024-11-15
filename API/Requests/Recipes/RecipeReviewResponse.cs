namespace API.Requests.Recipes;

public class RecipeReviewResponse
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? UserName { get; set; } // Optional: Include user's name in response
}
