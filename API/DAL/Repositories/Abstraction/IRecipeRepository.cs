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
    Task<IEnumerable<RecipesGet>> SearchRecipesByNameAsync(string searchTerm);
    Task<RecipeReviewResponse> AddRecipeReviewAsync(RecipeReview review);
    Task<IEnumerable<RecipeReviewResponse>> GetRecipeReviewsAsync(Guid recipeId);
    Task<bool> UpdateRecipeReviewAsync(Guid reviewId, RecipeReviewRequest review);
    Task<bool> DeleteRecipeReviewAsync(Guid reviewId);
    Task<RecipeReview?> GetRecipeReviewByIdAsync(Guid reviewId);
}
