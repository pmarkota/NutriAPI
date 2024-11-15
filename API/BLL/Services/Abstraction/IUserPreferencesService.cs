using API.DAL.Models;
using API.Requests.UserPreferences;

namespace API.BLL.Services.Abstraction;

public interface IUserPreferencesService : IService
{
    Task<UserPreference?> GetUserPreferencesAsync(Guid userId);
    Task<bool> UpdateUserPreferencesAsync(UpdateUserPreferencesRequest request);
    Task<bool> CreateUserPreferencesAsync(CreateUserPreferencesRequest request);
    Task<bool> DeleteUserPreferencesAsync(Guid userId);
    Task<bool> AddFavoriteRecipeAsync(Guid userId, Guid recipeId);
    Task<bool> RemoveFavoriteRecipeAsync(Guid userId, Guid recipeId);
    Task<bool> AddExcludedIngredientAsync(Guid userId, string ingredient);
    Task<bool> RemoveExcludedIngredientAsync(Guid userId, string ingredient);
}
