using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.BLL.Services.Abstraction;
using API.DAL.Models;
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
        private readonly IUserService _userService;

        public UsersController(IUserService userservice)
        {
            _userService = userservice;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }


// ...

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] UserRegisterRequest request)
        {
            try
            {
                var token = await _userService.RegisterUserAsync(request);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}