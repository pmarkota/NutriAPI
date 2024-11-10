using System.Text.Json.Serialization;

namespace API.DAL.Models;

public class GoogleUserInfo
{
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("sub")]
    public string GoogleId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("picture")]
    public string Picture { get; set; }
} 