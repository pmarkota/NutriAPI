namespace API.Requests.Recipes;

public class RecipeReviewResponse
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
