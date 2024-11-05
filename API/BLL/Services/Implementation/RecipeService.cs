using API.BLL.Services.Abstraction;
using API.DAL.Models;
using API.DAL.Repositories.Abstraction;
using API.Requests.Recipes;

namespace API.BLL.Services.Implementation;

public class RecipeService : IRecipeService, IService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public async Task<IEnumerable<RecipesGet>> GetAllRecipesAsync()
    {
        return await _recipeRepository.GetRecipesAsync();
    }

    public async Task<RecipesGet?> GetRecipeByIdAsync(Guid recipeId)
    {
        return await _recipeRepository.GetRecipeByIdAsync(recipeId);
    }

    public async Task AddRecipeAsync(RecipePostRequest recipe)
    {
        await _recipeRepository.AddRecipeAsync(recipe);
    }

    public async Task<bool> UpdateRecipeAsync(RecipePutRequest recipe)
    {
        return await _recipeRepository.UpdateRecipeAsync(recipe);
    }

    public async Task<bool> DeleteRecipeAsync(Guid recipeId)
    {
        return await _recipeRepository.DeleteRecipeAsync(recipeId);
    }
}