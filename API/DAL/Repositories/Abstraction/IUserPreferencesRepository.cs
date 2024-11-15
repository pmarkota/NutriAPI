using API.DAL.Models;

namespace API.DAL.Repositories.Abstraction;

public interface IUserPreferencesRepository : IRepository
{
    Task<UserPreference?> GetUserPreferencesAsync(Guid userId);
    Task<bool> UpdateUserPreferencesAsync(UserPreference preferences);
    Task<bool> CreateUserPreferencesAsync(UserPreference preferences);
    Task<bool> DeleteUserPreferencesAsync(Guid userId);
}
