namespace API.Requests.Users
{
    public class UserProfileResponse
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Goal { get; set; }
        public string? DietaryPreference { get; set; }
        public long? CaloricGoal { get; set; }
    }
}
