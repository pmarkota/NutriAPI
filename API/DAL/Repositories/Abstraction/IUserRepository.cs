using API.DAL.Models;

using API.Requests.Users;

using Microsoft.AspNetCore.Mvc;



namespace API.DAL.Repositories.Abstraction;



public interface IUserRepository

{

    Task<IEnumerable<User>> GetUsersAsync();

    Task<User> GetUserByIdAsync(Guid userId);

    Task<User> GetUserByUsernameOrEmailAsync(string username,string email);

    Task AddUserAsync(User user);

    Task<bool> UpdateUserAsync(UserProfileUpdateRequest request);

    Task SaveChangesAsync();

    Task<UserDietaryPreferences> GetUserDietaryPreferencesAsync(Guid userId);

    Task<User> GetUserByGoogleIdAsync(string googleId);

}
