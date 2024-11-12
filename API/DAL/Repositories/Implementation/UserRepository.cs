using API.DAL.Models;
using API.DAL.Repositories.Abstraction;
using API.Requests.Users;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Repositories.Implementation;

public class UserRepository : IUserRepository, IRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User> GetUserByUsernameOrEmailAsync(string username, string email) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Username == username || u.Email == email);

    public async Task AddUserAsync(User user) => await _db.Users.AddAsync(user);

    public async Task<bool> UpdateUserAsync(UserProfileUpdateRequest request)
    {
        var user = await _db.Users.FindAsync(request.UserId);

        if (user == null)
            return false;

        if (!string.IsNullOrEmpty(request.Username))
            user.Username = request.Username;

        if (!string.IsNullOrEmpty(request.Goal))
            user.Goal = request.Goal;

        if (!string.IsNullOrEmpty(request.DietaryPreference))
            user.DietaryPreference = request.DietaryPreference;

        if (request.CaloricGoal.HasValue)
            user.CaloricGoal = request.CaloricGoal;

        await _db.SaveChangesAsync();

        return true;
    }

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();

    public async Task<UserDietaryPreferences?> GetUserDietaryPreferencesAsync(Guid userId)
    {
        var user = await _db
            .Users.Where(u => u.Id == userId)
            .Select(u => new UserDietaryPreferences
            {
                DietaryPreference = u.DietaryPreference,

                Goal = u.Goal,

                CaloricGoal = u.CaloricGoal,
            })
            .FirstOrDefaultAsync();

        return user;
    }

    public async Task<User> GetUserByGoogleIdAsync(string googleId)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
    }
}
