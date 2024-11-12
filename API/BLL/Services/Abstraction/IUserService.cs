using API.DAL.Models;
using API.Requests.Users;
using Microsoft.AspNetCore.Mvc;

namespace API.BLL.Services.Abstraction;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();

    Task<User?> GetUserByIdAsync(Guid userId);

    Task<string> RegisterUserAsync(UserRegisterRequest request);

    Task<string> LoginUserAsync(UserLoginRequest request);

    Task<bool> UpdateUserAsync(UserProfileUpdateRequest request);

    Task<UserDietaryPreferences> GetUserDietaryPreferencesAsync(Guid userId);

    Task<string> HandleGoogleLoginAsync(GoogleUserInfo userInfo);

    Task<bool> CheckUserExistsByEmailAsync(string email);
    Task<string> HandleExistingGoogleLoginAsync(GoogleLoginExistingRequest request);

    Task<UserProfileResponse?> GetUserProfileAsync(Guid userId);
}
