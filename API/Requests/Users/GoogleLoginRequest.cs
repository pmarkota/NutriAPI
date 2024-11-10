namespace API.Requests.Users;
using System.Text.Json.Serialization;

public class GoogleLoginRequest
{
    [JsonPropertyName("AccessToken")]
    public string AccessToken { get; set; }
    
    [JsonPropertyName("email")]
    public string Email { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("picture")]
    public string Picture { get; set; }
} 
