namespace API.Requests.MealPlans;

public class MealPlanGenerateRequest
{
    public Guid UserId { get; set; }
    public int DurationInDays { get; set; } = 7; // Default to a week
    public bool ConsiderUserPreferences { get; set; } = true;
    public string? SpecificDietaryPreference { get; set; } // Override user preferences if specified
    public long? SpecificCaloricGoal { get; set; } // Override user caloric goal if specified
}
