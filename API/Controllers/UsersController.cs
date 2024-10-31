using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Models;
using API.Requests.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return _db.Users.ToList();
        }


        [HttpGet("{id}")]
        public ActionResult<User> GetUser(Guid id)
        {
            var user = _db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }


// ...

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] UserRegisterRequest request)
        {
            // Check if user with the same username or email already exists
            var existingUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);

            if (existingUser != null)
            {
                return Conflict("Username or email already exists.");
            }

            // Hash the password
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            var passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)));

            // Create new user object
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

            // Add the new user to the database
            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            // Create claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, newUser.Id.ToString())
            };

            // Generate JWT token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                "fcecb0e3e04ffe3a94cbbe4453c4c7679a3028d931843da12fde1257760a6020f56450b6f4deb18ad149a61f7a22d10e68afdae763efcade033b344a984ee907+6zUjv7hK7IoCp5i3C6tTg3aFf1mE=")); // Use a secure key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                issuer: "changethislater.com",
                audience: "changethislater.com",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(jwt);
        }
    }
}