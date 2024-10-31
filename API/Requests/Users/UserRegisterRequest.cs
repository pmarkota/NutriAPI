namespace API.Requests.Users;

public class UserRegisterRequest
{
    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Goal { get; set; }

    public string? DietaryPreference { get; set; }

    public long? CaloricGoal { get; set; }


    public DateTime? LastLogin { get; set; }

}