using API.Requests.Recipes;

namespace API.BLL.Services.Abstraction;

public interface IRecipeService
{
    Task<IEnumerable<RecipesGet>> GetAllRecipesAsync();
    Task<RecipesGet?> GetRecipeByIdAsync(Guid recipeId);
    Task AddRecipeAsync(RecipePostRequest recipe);
    Task<bool> UpdateRecipeAsync(RecipePutRequest recipe);
    Task<bool> DeleteRecipeAsync(Guid recipeId);
}
