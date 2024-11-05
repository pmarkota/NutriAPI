using API.DAL.Models;
using API.DAL.Repositories.Abstraction;
using API.Requests.Users;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Repositories.Implementation;

public class UserRepository : IUserRepository,IRepository
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

    public async Task<User> GetUserByUsernameOrEmailAsync(string username, string email)=>
        await _db.Users.FirstOrDefaultAsync(u => u.Username == username || u.Email == email);

    public async Task AddUserAsync(User user) => await _db.Users.AddAsync(user);
    public async Task<bool> UpdateUserAsync(UserProfileUpdateRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.Id);
        if (user == null)
        {
            return false; // Return false if user not found
        }

        user.DietaryPreference = request.DietaryPreference;
        user.Goal = request.Goal;
        user.CaloricGoal = request.CaloricGoal;

        await _db.SaveChangesAsync();
        return true; // Return true if update was successful
    }



    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    public async Task<UserDietaryPreferences?> GetUserDietaryPreferencesAsync(Guid userId)
    {
        var user = await _db.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserDietaryPreferences
            {
                DietaryPreference = u.DietaryPreference,
                Goal = u.Goal,
                CaloricGoal = u.CaloricGoal
            })
            .FirstOrDefaultAsync();

        return user;
    }

}