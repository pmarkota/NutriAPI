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
    Task<IActionResult> UpdateUserAsync(UserProfileUpdateRequest request);

}