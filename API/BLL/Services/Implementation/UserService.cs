using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.BLL.Services.Abstraction;
using API.DAL.Models;
using API.DAL.Repositories.Abstraction;
using API.Requests.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.BLL.Services.Implementation;

public class UserService : IUserService, IService
{
    private readonly IUserRepository _userRepository;

    private readonly IConfiguration _configuration;

    public UserService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;

        _configuration = configuration;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync() =>
        await _userRepository.GetUsersAsync();

    public async Task<User> GetUserByIdAsync(Guid id) => await _userRepository.GetUserByIdAsync(id);

    public async Task<string> RegisterUserAsync(UserRegisterRequest request)
    {
        var existingUser = await _userRepository.GetUserByUsernameOrEmailAsync(
            request.Username,
            request.Email
        );

        if (existingUser != null)
            throw new Exception("Username or email already exists.");

        var passwordHash = HashPassword(request.Password);

        var newUser = new User
        {
            Username = request.Username,

            Email = request.Email,

            PasswordHash = passwordHash,

            CreatedAt = DateTime.UtcNow,

            Goal = request.Goal,

            DietaryPreference = request.DietaryPreference,

            CaloricGoal = request.CaloricGoal,

            LastLogin = DateTime.UtcNow,
        };

        await _userRepository.AddUserAsync(newUser);

        await _userRepository.SaveChangesAsync();

        return GenerateJwtToken(newUser.Id);
    }

    public async Task<string> LoginUserAsync(UserLoginRequest request)
    {
        var user = await _userRepository.GetUserByUsernameOrEmailAsync(
            request.Username,
            request.Email
        );

        if (user == null || !VerifyPasswordHash(request.Password, user.PasswordHash))
        {
            throw new Exception("Invalid username/email or password.");
        }

        user.LastLogin = DateTime.UtcNow;

        await _userRepository.SaveChangesAsync();

        return GenerateJwtToken(user.Id);
    }

    public async Task<bool> UpdateUserAsync(UserProfileUpdateRequest request)
    {
        // If username is being changed, check if new username is available
        if (!string.IsNullOrEmpty(request.Username))
        {
            var existingUser = await _userRepository.GetUserByUsernameOrEmailAsync(
                request.Username,
                null
            );
            if (existingUser != null && existingUser.Id != request.UserId)
            {
                throw new Exception("Username already taken");
            }
        }

        return await _userRepository.UpdateUserAsync(request);
    }

    public async Task<UserDietaryPreferences?> GetUserDietaryPreferencesAsync(Guid userId)
    {
        return await _userRepository.GetUserDietaryPreferencesAsync(userId);
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPasswordHash(string requestPassword, string userPasswordHash)
    {
        return BCrypt.Net.BCrypt.Verify(requestPassword, userPasswordHash);
    }

    private string GenerateJwtToken(Guid userId)
    {
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()) };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"])
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateUsername(string fullName)
    {
        // Split the name into parts



        var nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Get first and last name, or use the whole name if only one part



        var firstName = nameParts.FirstOrDefault() ?? "";

        var lastName = nameParts.Length > 1 ? nameParts[^1] : "";

        // Generate random numbers between 10-99



        var random = new Random();

        var randomNumbers = random.Next(10, 100);

        // Combine parts and remove any special characters



        var username = $"{firstName}{lastName}{randomNumbers}"
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("'", "");

        return username;
    }

    public async Task<string> HandleGoogleLoginAsync(GoogleUserInfo userInfo)
    {
        var user = await _userRepository.GetUserByGoogleIdAsync(userInfo.GoogleId);

        if (user == null)
        {
            var passwordHash = HashPassword(userInfo.Password);

            var username = GenerateUsername(userInfo.Name); // Generate username from Google name

            user = new User
            {
                Email = userInfo.Email,

                Username = username, // Use generated username instead of email

                GoogleId = userInfo.GoogleId,

                Picture = userInfo.Picture,

                PasswordHash = passwordHash,

                CreatedAt = DateTime.UtcNow,

                LastLogin = DateTime.UtcNow,
            };

            await _userRepository.AddUserAsync(user);

            await _userRepository.SaveChangesAsync();
        }
        else
        {
            // Update existing user



            user.LastLogin = DateTime.UtcNow;

            user.Picture = userInfo.Picture;

            await _userRepository.SaveChangesAsync();
        }

        return GenerateJwtToken(user.Id);
    }

    public async Task<bool> CheckUserExistsByEmailAsync(string email)
    {
        var user = await _userRepository.GetUserByUsernameOrEmailAsync(null, email);

        return user != null;
    }

    public async Task<string> HandleExistingGoogleLoginAsync(GoogleLoginExistingRequest request)
    {
        // Verify the Google token and get user info



        using var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.AccessToken);

        var userInfoResponse = await httpClient.GetAsync(
            "https://www.googleapis.com/oauth2/v3/userinfo"
        );

        if (!userInfoResponse.IsSuccessStatusCode)
        {
            throw new Exception("Failed to verify Google token");
        }

        var user = await _userRepository.GetUserByUsernameOrEmailAsync(null, request.Email);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        user.LastLogin = DateTime.UtcNow;

        await _userRepository.SaveChangesAsync();

        return GenerateJwtToken(user.Id);
    }

    public async Task<UserProfileResponse?> GetUserProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        return new UserProfileResponse
        {
            Username = user.Username,
            Email = user.Email,
            Goal = user.Goal,
            DietaryPreference = user.DietaryPreference,
            CaloricGoal = user.CaloricGoal,
        };
    }
}
