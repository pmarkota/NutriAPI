using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourNamespace.Models.Requests
{
    public class RecipeFilterRequest
    {
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public string? Difficulty { get; set; }
        public int? MaxPrepTime { get; set; }
        public int? MaxCookingTime { get; set; }
        public int? MaxTotalTime { get; set; }
        public long? MaxCalories { get; set; }
        public string? DietaryLabels { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
} 