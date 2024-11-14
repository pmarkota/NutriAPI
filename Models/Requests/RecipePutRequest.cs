using System.ComponentModel.DataAnnotations;

namespace YourNamespace.Models.Requests
{
    public class RecipePutRequest : RecipePostRequest
    {
        [Required]
        public Guid Id { get; set; }
    }
} 