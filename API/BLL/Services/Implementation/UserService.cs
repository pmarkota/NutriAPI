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



    public async Task<IEnumerable<User>> GetAllUsersAsync() => await _userRepository.GetUsersAsync();



    public async Task<User> GetUserByIdAsync(Guid id) => await _userRepository.GetUserByIdAsync(id);



    public async Task<string> RegisterUserAsync(UserRegisterRequest request)

    {

        var existingUser = await _userRepository.GetUserByUsernameOrEmailAsync(request.Username, request.Email);

        if (existingUser != null) throw new Exception("Username or email already exists.");



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

            LastLogin = DateTime.UtcNow

        };



        await _userRepository.AddUserAsync(newUser);

        await _userRepository.SaveChangesAsync();



        return GenerateJwtToken(newUser.Id);

    }



    public async Task<string> LoginUserAsync(UserLoginRequest request)

    {

        var user = await _userRepository.GetUserByUsernameOrEmailAsync(request.Username, request.Email);

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



        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);



        var token = new JwtSecurityToken(

            issuer: _configuration["JwtSettings:Issuer"],

            audience: _configuration["JwtSettings:Audience"],

            claims: claims,

            expires: DateTime.UtcNow.AddDays(1),

            signingCredentials: creds);



        return new JwtSecurityTokenHandler().WriteToken(token);

    }



    public async Task<string> HandleGoogleLoginAsync(GoogleUserInfo userInfo)

    {

        var user = await _userRepository.GetUserByGoogleIdAsync(userInfo.GoogleId);

        

        if (user == null)

        {

            // Create new user

            user = new User

            {

                Email = userInfo.Email,

                Username = userInfo.Email, // You might want to generate a username differently

                GoogleId = userInfo.GoogleId,

                Picture = userInfo.Picture,

                CreatedAt = DateTime.UtcNow,

                LastLogin = DateTime.UtcNow

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

}
