using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class MealPlanRecipe
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("meal_plan_id")]
    public Guid? MealPlanId { get; set; }

    [Column("recipe_id")]
    public Guid? RecipeId { get; set; }

    [Column("day", TypeName = "character varying")]
    public string? Day { get; set; }

    [Column("meal_type", TypeName = "character varying")]
    public string? MealType { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("MealPlanId")]
    [InverseProperty("MealPlanRecipes")]
    public virtual MealPlan? MealPlan { get; set; }

    [ForeignKey("RecipeId")]
    [InverseProperty("MealPlanRecipes")]
    public virtual Recipe? Recipe { get; set; }
}
