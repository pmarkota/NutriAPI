using API.DAL.Models;
using API.Requests.Recipes;

namespace API.DAL.Repositories.Abstraction;

public interface IRecipeRepository
{
    Task<IEnumerable<RecipesGet>> GetRecipesAsync();
    Task<RecipesGet?> GetRecipeByIdAsync(Guid recipeId);
    Task<RecipesGet> AddRecipeAsync(RecipePostRequest recipe);
    Task<bool> UpdateRecipeAsync(RecipePutRequest recipe);
    Task<bool> DeleteRecipeAsync(Guid recipeId);
    Task<IEnumerable<RecipesGet>> GetFilteredRecipesAsync(RecipeFilterRequest filter);
}
