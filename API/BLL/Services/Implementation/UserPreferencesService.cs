using System.Text.Json;
using API.BLL.Services.Abstraction;
using API.DAL.Models;
using API.DAL.Repositories.Abstraction;
using API.Requests.UserPreferences;

namespace API.BLL.Services.Implementation;

public class UserPreferencesService : IUserPreferencesService, IService
{
    private readonly IUserPreferencesRepository _userPreferencesRepository;

    public UserPreferencesService(IUserPreferencesRepository userPreferencesRepository)
    {
        _userPreferencesRepository = userPreferencesRepository;
    }

    private async Task<UserPreference?> GetOrCreateUserPreferencesAsync(Guid userId)
    {
        var preferences = await _userPreferencesRepository.GetUserPreferencesAsync(userId);

        if (preferences == null)
        {
            var newPreferences = new UserPreference
            {
                UserId = userId,
                FavoriteRecipes = "[]",
                ExcludedIngredients = "[]",
            };

            var created = await _userPreferencesRepository.CreateUserPreferencesAsync(
                newPreferences
            );
            return created ? newPreferences : null;
        }

        return preferences;
    }

    public async Task<UserPreference?> GetUserPreferencesAsync(Guid userId)
    {
        return await GetOrCreateUserPreferencesAsync(userId);
    }

    public async Task<bool> UpdateUserPreferencesAsync(UpdateUserPreferencesRequest request)
    {
        var preferences = new UserPreference
        {
            UserId = request.UserId,
            FavoriteRecipes = request.FavoriteRecipes,
            ExcludedIngredients = request.ExcludedIngredients,
        };

        return await _userPreferencesRepository.UpdateUserPreferencesAsync(preferences);
    }

    public async Task<bool> CreateUserPreferencesAsync(CreateUserPreferencesRequest request)
    {
        var preferences = new UserPreference
        {
            UserId = request.UserId,
            FavoriteRecipes = request.FavoriteRecipes,
            ExcludedIngredients = request.ExcludedIngredients,
        };

        return await _userPreferencesRepository.CreateUserPreferencesAsync(preferences);
    }

    public async Task<bool> DeleteUserPreferencesAsync(Guid userId)
    {
        return await _userPreferencesRepository.DeleteUserPreferencesAsync(userId);
    }

    public async Task<bool> AddFavoriteRecipeAsync(Guid userId, Guid recipeId)
    {
        var preferences = await GetOrCreateUserPreferencesAsync(userId);
        if (preferences == null)
            return false;

        var favoriteRecipes =
            JsonSerializer.Deserialize<List<Guid>>(preferences.FavoriteRecipes) ?? new List<Guid>();
        if (!favoriteRecipes.Contains(recipeId))
        {
            favoriteRecipes.Add(recipeId);
            preferences.FavoriteRecipes = JsonSerializer.Serialize(favoriteRecipes);
            return await _userPreferencesRepository.UpdateUserPreferencesAsync(preferences);
        }
        return true;
    }

    public async Task<bool> RemoveFavoriteRecipeAsync(Guid userId, Guid recipeId)
    {
        var preferences = await GetOrCreateUserPreferencesAsync(userId);
        if (preferences == null)
            return false;

        var favoriteRecipes =
            JsonSerializer.Deserialize<List<Guid>>(preferences.FavoriteRecipes) ?? new List<Guid>();
        if (favoriteRecipes.Remove(recipeId))
        {
            preferences.FavoriteRecipes = JsonSerializer.Serialize(favoriteRecipes);
            return await _userPreferencesRepository.UpdateUserPreferencesAsync(preferences);
        }
        return true;
    }

    public async Task<bool> AddExcludedIngredientAsync(Guid userId, string ingredient)
    {
        var preferences = await GetOrCreateUserPreferencesAsync(userId);
        if (preferences == null)
            return false;

        var excludedIngredients =
            JsonSerializer.Deserialize<List<string>>(preferences.ExcludedIngredients ?? "[]")
            ?? new List<string>();

        if (!excludedIngredients.Contains(ingredient, StringComparer.OrdinalIgnoreCase))
        {
            excludedIngredients.Add(ingredient);
            preferences.ExcludedIngredients = JsonSerializer.Serialize(excludedIngredients);
            return await _userPreferencesRepository.UpdateUserPreferencesAsync(preferences);
        }
        return true;
    }

    public async Task<bool> RemoveExcludedIngredientAsync(Guid userId, string ingredient)
    {
        var preferences = await GetOrCreateUserPreferencesAsync(userId);
        if (preferences == null)
            return false;

        var excludedIngredients =
            JsonSerializer.Deserialize<List<string>>(preferences.ExcludedIngredients ?? "[]")
            ?? new List<string>();

        if (
            excludedIngredients.RemoveAll(x =>
                x.Equals(ingredient, StringComparison.OrdinalIgnoreCase)
            ) > 0
        )
        {
            preferences.ExcludedIngredients = JsonSerializer.Serialize(excludedIngredients);
            return await _userPreferencesRepository.UpdateUserPreferencesAsync(preferences);
        }
        return true;
    }
}
