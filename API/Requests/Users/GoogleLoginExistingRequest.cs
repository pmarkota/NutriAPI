namespace API.Requests.Users;

public class GoogleLoginExistingRequest
{
    public string AccessToken { get; set; }
    public string Email { get; set; }
}