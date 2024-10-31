using API.DAL.Models;
using API.Requests.Users;

namespace API.BLL.Services.Abstraction;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<string> RegisterUserAsync(UserRegisterRequest request);    
}