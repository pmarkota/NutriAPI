using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace API.DAL.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public virtual DbSet<MealPlan> MealPlans { get; set; }
    public virtual DbSet<RecipeReview> RecipeReviews { get; set; }

    public virtual DbSet<MealPlanRecipe> MealPlanRecipes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<ShoppingList> ShoppingLists { get; set; }

    public virtual DbSet<ShoppingListItem> ShoppingListItems { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPreference> UserPreferences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(GetConnectionString());
        }
    }

    private string GetConnectionString()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        return configuration.GetConnectionString("DefaultConnection");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set the default schema







        modelBuilder.HasDefaultSchema("public");

        // Configure the User entity







        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", "public");

            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<MealPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("MealPlans_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.MealPlans)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("MealPlans_user_id_fkey");
        });

        modelBuilder.Entity<MealPlanRecipe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("MealPlanRecipes_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

            entity
                .HasOne(d => d.MealPlan)
                .WithMany(p => p.MealPlanRecipes)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("MealPlanRecipes_meal_plan_id_fkey");

            entity
                .HasOne(d => d.Recipe)
                .WithMany(p => p.MealPlanRecipes)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("MealPlanRecipes_recipe_id_fkey");
        });
        modelBuilder.Entity<RecipeReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("recipereviews_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");

            entity.HasCheckConstraint(
                "RecipeReviews_rating_check",
                "(rating >= 1) AND (rating <= 5)"
            );

            entity
                .HasOne(d => d.Recipe)
                .WithMany(p => p.RecipeReviews)
                .HasForeignKey(d => d.RecipeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("recipereviews_recipe_id_fkey");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.RecipeReviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("recipereviews_user_id_fkey");
        });
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Notifications_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.IsSent).HasDefaultValue(false);

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.Notifications)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Notifications_user_id_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Payments_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Payments_user_id_fkey");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Recipes_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

            entity
                .HasOne(d => d.CreatedByNavigation)
                .WithMany(p => p.Recipes)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Recipes_created_by_fkey");
        });

        modelBuilder.Entity<ShoppingList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ShoppingLists_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.GeneratedAt).HasDefaultValueSql("now()");

            entity
                .HasOne(d => d.MealPlan)
                .WithMany(p => p.ShoppingLists)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ShoppingLists_meal_plan_id_fkey");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.ShoppingLists)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ShoppingLists_user_id_fkey");
        });

        modelBuilder.Entity<ShoppingListItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ShoppingListItems_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.IsChecked).HasDefaultValue(false);

            entity
                .HasOne(d => d.ShoppingList)
                .WithMany(p => p.ShoppingListItems)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ShoppingListItems_shopping_list_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<UserPreference>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("UserPreferences_pkey");

            entity.Property(e => e.UserId).HasDefaultValueSql("gen_random_uuid()");

            entity
                .HasOne(d => d.User)
                .WithOne(p => p.UserPreference)
                .HasConstraintName("UserPreferences_user_id_fkey");
        });

        modelBuilder.HasSequence<int>("seq_schema_version", "graphql").IsCyclic();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
