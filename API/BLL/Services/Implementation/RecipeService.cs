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

    public async Task<RecipesGet> AddRecipeAsync(RecipePostRequest recipe)
    {
        if (String.IsNullOrEmpty(recipe.Name))
            throw new ArgumentException("Recipe name is required");

        return await _recipeRepository.AddRecipeAsync(recipe);
    }

    public async Task<bool> UpdateRecipeAsync(RecipePutRequest recipe)
    {
        return await _recipeRepository.UpdateRecipeAsync(recipe);
    }

    public async Task<bool> DeleteRecipeAsync(Guid recipeId)
    {
        return await _recipeRepository.DeleteRecipeAsync(recipeId);
    }

    public async Task<IEnumerable<RecipesGet>> GetFilteredRecipesAsync(RecipeFilterRequest filter)
    {
        if (filter.MaxCalories.HasValue && filter.MaxCalories < 0)
            throw new ArgumentException("MaxCalories cannot be negative");

        if (filter.MinProtein.HasValue && filter.MinProtein < 0)
            throw new ArgumentException("MinProtein cannot be negative");

        if (filter.MaxCarbohydrates.HasValue && filter.MaxCarbohydrates < 0)
            throw new ArgumentException("MaxCarbohydrates cannot be negative");

        if (filter.MaxFats.HasValue && filter.MaxFats < 0)
            throw new ArgumentException("MaxFats cannot be negative");

        return await _recipeRepository.GetFilteredRecipesAsync(filter);
    }

    public async Task<IEnumerable<RecipesGet>> SearchRecipesByNameAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new ArgumentException("Search term cannot be empty");

        return await _recipeRepository.SearchRecipesByNameAsync(searchTerm);
    }
}
