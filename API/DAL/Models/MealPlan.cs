using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Models;

public partial class MealPlan
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("plan_name", TypeName = "character varying")]
    public string? PlanName { get; set; }

    [Column("start_date")]
    public DateOnly? StartDate { get; set; }

    [Column("end_date")]
    public DateOnly? EndDate { get; set; }

    [Column("caloric_total")]
    public long? CaloricTotal { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("MealPlan")]
    public virtual ICollection<MealPlanRecipe> MealPlanRecipes { get; set; } = new List<MealPlanRecipe>();

    [InverseProperty("MealPlan")]
    public virtual ICollection<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();

    [ForeignKey("UserId")]
    [InverseProperty("MealPlans")]
    public virtual User? User { get; set; }
}
