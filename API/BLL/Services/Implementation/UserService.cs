using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.BLL.Services.Abstraction;
using API.DAL.Models;
using API.DAL.Repositories.Abstraction;
using API.Requests.Users;
using Microsoft.IdentityModel.Tokens;

namespace API.BLL.Services.Implementation;

public class UserService : IUserService, IService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync() => await _userRepository.GetUsersAsync();


    public async Task<User> GetUserByIdAsync(Guid id) => await _userRepository.GetUserByIdAsync(id);


    public async Task<string> RegisterUserAsync(UserRegisterRequest request)
    {
        var existingUser = await _userRepository.GetUserByUsernameOrEmailAsync(request.Username, request.Email);
        if (existingUser != null) throw new Exception("Username or email already exists.");

        using var hmac = new System.Security.Cryptography.HMACSHA512();
        var passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)));

        var newUser = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            Goal = request.Goal,
            DietaryPreference = request.DietaryPreference,
            CaloricGoal = request.CaloricGoal,
            LastLogin = DateTime.UtcNow
        };

        await _userRepository.AddUserAsync(newUser);
        await _userRepository.SaveChangesAsync();

        // Generate JWT
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, newUser.Id.ToString()) };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            "fcecb0e3e04ffe3a94cbbe4453c4c7679a3028d931843da12fde1257760a6020f56450b6f4deb18ad149a61f7a22d10e68afdae763efcade033b344a984ee907+6zUjv7hK7IoCp5i3C6tTg3aFf1mE="));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            issuer: "mysite.com",
            audience: "mysite.com",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}