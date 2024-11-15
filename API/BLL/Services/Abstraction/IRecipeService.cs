using API.Requests.Recipes;

namespace API.BLL.Services.Abstraction;

public interface IRecipeService
{
    Task<IEnumerable<RecipesGet>> GetAllRecipesAsync();
    Task<RecipesGet?> GetRecipeByIdAsync(Guid recipeId);
    Task<RecipesGet> AddRecipeAsync(RecipePostRequest recipe);
    Task<bool> UpdateRecipeAsync(RecipePutRequest recipe);
    Task<bool> DeleteRecipeAsync(Guid recipeId);
    Task<IEnumerable<RecipesGet>> GetFilteredRecipesAsync(RecipeFilterRequest filter);
    Task<IEnumerable<RecipesGet>> SearchRecipesByNameAsync(string searchTerm);
}
