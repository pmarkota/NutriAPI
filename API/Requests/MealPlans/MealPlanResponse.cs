namespace API.Requests.MealPlans;

public class MealPlanResponse
{
    public Guid Id { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public long TotalCalories { get; set; }
    public long? TotalProtein { get; set; }
    public long? TotalCarbohydrates { get; set; }
    public long? TotalFats { get; set; }
    public List<DailyMealPlan> DailyPlans { get; set; } = new();
}

public class DailyMealPlan
{
    public DateOnly Date { get; set; }
    public string Day { get; set; } = string.Empty;
    public List<MealPlanRecipeInfo> Meals { get; set; } = new();
    public long DailyTotalCalories { get; set; }
    public long? DailyTotalProtein { get; set; }
    public long? DailyTotalCarbohydrates { get; set; }
    public long? DailyTotalFats { get; set; }
}

public class MealPlanRecipeInfo
{
    public Guid RecipeId { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public string MealType { get; set; } = string.Empty; // Breakfast, Lunch, Dinner, Snack
    public long Calories { get; set; }
    public long? Protein { get; set; }
    public long? Carbohydrates { get; set; }
    public long? Fats { get; set; }
}
