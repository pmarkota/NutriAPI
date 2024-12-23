namespace API.Requests.UserPreferences;

public class CreateUserPreferencesRequest
{
    public Guid UserId { get; set; }
    public string FavoriteRecipes { get; set; } = null!;
    public string? ExcludedIngredients { get; set; }
}
