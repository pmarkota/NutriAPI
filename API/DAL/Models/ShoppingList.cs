using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Models;

public partial class ShoppingList
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("meal_plan_id")]
    public Guid? MealPlanId { get; set; }

    [Column("generated_at")]
    public DateTime GeneratedAt { get; set; }

    [ForeignKey("MealPlanId")]
    [InverseProperty("ShoppingLists")]
    public virtual MealPlan? MealPlan { get; set; }

    [InverseProperty("ShoppingList")]
    public virtual ICollection<ShoppingListItem> ShoppingListItems { get; set; } =
        new List<ShoppingListItem>();

    [ForeignKey("UserId")]
    [InverseProperty("ShoppingLists")]
    public virtual User? User { get; set; }
}
