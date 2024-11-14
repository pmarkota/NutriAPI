using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace YourNamespace.Models.Requests
{
    public class RecipePostRequest
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public JsonDocument Ingredients { get; set; }
        [Required]
        public string Instructions { get; set; }
        public long? Calories { get; set; }
        public long? Protein { get; set; }
        public long? Carbohydrates { get; set; }
        public long? Fats { get; set; }
        public string? DietaryLabels { get; set; }
        public string? Category { get; set; }
        public string? Difficulty { get; set; }
        public int? PrepTime { get; set; }
        public int? CookingTime { get; set; }
    }
} 