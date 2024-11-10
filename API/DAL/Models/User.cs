using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Models;

public partial class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("username", TypeName = "character varying")]
    public string? Username { get; set; }

    [Column("email", TypeName = "character varying")]
    public string? Email { get; set; }

    [Column("password_hash")]
    public string? PasswordHash { get; set; }

    [Column("goal", TypeName = "character varying")]
    public string? Goal { get; set; }

    [Column("dietary_preference")]
    public string? DietaryPreference { get; set; }

    [Column("caloric_goal")]
    public long? CaloricGoal { get; set; }

    [Column("subscription_type", TypeName = "character varying")]
    public string? SubscriptionType { get; set; }

    [Column("last_login")]
    public DateTime? LastLogin { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("google_id")]
    public string? GoogleId { get; set; }

    [Column("picture")]
    public string? Picture { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();

    [InverseProperty("User")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("User")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    [InverseProperty("User")]
    public virtual ICollection<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();

    [InverseProperty("User")]
    public virtual UserPreference? UserPreference { get; set; }
}














