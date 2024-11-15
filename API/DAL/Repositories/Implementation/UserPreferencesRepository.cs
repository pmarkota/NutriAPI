using API.DAL.Models;
using API.DAL.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Repositories.Implementation;

public class UserPreferencesRepository : IUserPreferencesRepository, IRepository
{
    private readonly AppDbContext _db;

    public UserPreferencesRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<UserPreference?> GetUserPreferencesAsync(Guid userId)
    {
        return await _db.UserPreferences.FirstOrDefaultAsync(up => up.UserId == userId);
    }

    public async Task<bool> UpdateUserPreferencesAsync(UserPreference preferences)
    {
        var existing = await _db.UserPreferences.FirstOrDefaultAsync(up =>
            up.UserId == preferences.UserId
        );

        if (existing == null)
            return false;

        existing.FavoriteRecipes = preferences.FavoriteRecipes;
        existing.ExcludedIngredients = preferences.ExcludedIngredients;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CreateUserPreferencesAsync(UserPreference preferences)
    {
        await _db.UserPreferences.AddAsync(preferences);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserPreferencesAsync(Guid userId)
    {
        var preferences = await _db.UserPreferences.FirstOrDefaultAsync(up => up.UserId == userId);

        if (preferences == null)
            return false;

        _db.UserPreferences.Remove(preferences);
        await _db.SaveChangesAsync();
        return true;
    }
}
