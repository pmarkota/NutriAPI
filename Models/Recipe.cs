using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace YourNamespace.Models
{
    [Table("Recipes")]
    public class Recipe
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("ingredients")]
        public JsonDocument? Ingredients { get; set; }

        [Column("instructions")]
        public string? Instructions { get; set; }

        [Column("calories")]
        public long? Calories { get; set; }

        [Column("protein")]
        public long? Protein { get; set; }

        [Column("carbohydrates")]
        public long? Carbohydrates { get; set; }

        [Column("fats")]
        public long? Fats { get; set; }

        [Column("dietary_labels")]
        public string? DietaryLabels { get; set; }

        [Column("created_by")]
        public Guid? CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("category")]
        public string? Category { get; set; }

        [Column("difficulty")]
        public string? Difficulty { get; set; }

        [Column("prep_time")]
        public int? PrepTime { get; set; }

        [Column("cooking_time")]
        public int? CookingTime { get; set; }

        [Column("total_time")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? TotalTime { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User? User { get; set; }
    }
} 