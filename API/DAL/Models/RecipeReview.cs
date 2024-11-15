using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Models;

public partial class RecipeReview
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("recipe_id")]
    public Guid RecipeId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("rating")]
    public int Rating { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("RecipeId")]
    [InverseProperty("RecipeReviews")]
    public virtual Recipe Recipe { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("RecipeReviews")]
    public virtual User User { get; set; } = null!;
}
