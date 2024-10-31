using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Models;

public partial class UserPreference
{
    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("favorite_recipes")]
    public string FavoriteRecipes { get; set; } = null!;

    [Column("excluded_ingredients")]
    public string? ExcludedIngredients { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserPreference")]
    public virtual User User { get; set; } = null!;
}
