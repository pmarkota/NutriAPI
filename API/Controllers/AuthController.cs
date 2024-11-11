using API.BLL.Services.Abstraction;

using API.DAL.Models;

using API.Requests.Users;

using Google.Apis.Auth;

using Microsoft.AspNetCore.Mvc;



namespace API.Controllers;



[Route("api/[controller]")]

[ApiController]

public class AuthController : ControllerBase

{

    private readonly IUserService _userService;

    private readonly IConfiguration _configuration;



    public AuthController(IUserService userService, IConfiguration configuration)

    {

        _userService = userService;

        _configuration = configuration;

    }



    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            // First, get user info from Google using the access token
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.AccessToken);
        
            var userInfoResponse = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
            if (!userInfoResponse.IsSuccessStatusCode)
            {
                return BadRequest(new { message = "Failed to get user info from Google" });
            }

            var googleUserJson = await userInfoResponse.Content.ReadAsStringAsync();
            var googleUser = System.Text.Json.JsonSerializer.Deserialize<GoogleUserInfo>(googleUserJson);

            if (googleUser == null)
            {
                return BadRequest(new { message = "Invalid user info from Google" });
            }

            var userInfo = new GoogleUserInfo
            {
                AccessToken = request.AccessToken,
                Email = googleUser.Email,
                GoogleId = googleUser.GoogleId,  // This will be the sub claim from Google
                Name = googleUser.Name,
                Picture = googleUser.Picture,
                Password = request.Password  // Add this line to pass the password
            };

            var token = await _userService.HandleGoogleLoginAsync(userInfo);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Invalid Google token", error = ex.Message });
        }
    }
    [HttpPost("google/check-email")]
    public async Task<IActionResult> CheckGoogleEmail([FromBody] CheckEmailRequest request)
    {
        try
        {
            var userExists = await _userService.CheckUserExistsByEmailAsync(request.Email);
            // Always return Ok (200) with the exists flag
            return Ok(new { exists = userExists });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error checking email", error = ex.Message });
        }
    }

    [HttpPost("google/login")]
    public async Task<IActionResult> GoogleLoginExisting([FromBody] GoogleLoginExistingRequest request)
    {
        try
        {
            var token = await _userService.HandleExistingGoogleLoginAsync(request);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Login failed", error = ex.Message });
        }
    }
} 
