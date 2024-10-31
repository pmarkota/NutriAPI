using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Models;

public partial class Recipe
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name", TypeName = "character varying")]
    public string? Name { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("ingredients", TypeName = "jsonb")]
    public string? Ingredients { get; set; }

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

    [Column("dietary_labels", TypeName = "character varying")]
    public string? DietaryLabels { get; set; }

    [Column("created_by")]
    public Guid? CreatedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("Recipes")]
    public virtual User? CreatedByNavigation { get; set; }

    [InverseProperty("Recipe")]
    public virtual ICollection<MealPlanRecipe> MealPlanRecipes { get; set; } = new List<MealPlanRecipe>();
}
