﻿using System.Text.Json;
using API.DAL.Models;
using API.DAL.Repositories.Abstraction;
using API.Requests.Recipes;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Repositories.Implementation;

public class RecipeRepository : IRecipeRepository, IRepository
{
    private readonly AppDbContext _db;

    public RecipeRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<RecipesGet>> GetRecipesAsync()
    {
        return await _db
            .Recipes.Select(r => new RecipesGet
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Ingredients = r.Ingredients,
                Instructions = r.Instructions,
                Calories = r.Calories,
                Protein = r.Protein,
                Carbohydrates = r.Carbohydrates,
                Fats = r.Fats,
                DietaryLabels = r.DietaryLabels,
                CreatedBy = r.CreatedBy,
                CreatedAt = r.CreatedAt,
            })
            .ToListAsync();
    }

    public async Task<RecipesGet?> GetRecipeByIdAsync(Guid recipeId)
    {
        return await _db
            .Recipes.Where(r => r.Id == recipeId)
            .Select(r => new RecipesGet
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Ingredients = r.Ingredients,
                Instructions = r.Instructions,
                Calories = r.Calories,
                Protein = r.Protein,
                Carbohydrates = r.Carbohydrates,
                Fats = r.Fats,
                DietaryLabels = r.DietaryLabels,
                CreatedBy = r.CreatedBy,
                CreatedAt = r.CreatedAt,
                Category = r.Category,
                Difficulty = r.Difficulty,
                PrepTime = r.PrepTime,
                CookingTime = r.CookingTime,
                TotalTime = r.TotalTime,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<RecipesGet> AddRecipeAsync(RecipePostRequest recipe)
    {
        var newRecipe = new Recipe
        {
            Name = recipe.Name,
            Description = recipe.Description,
            Ingredients =
                recipe.Ingredients != null ? JsonSerializer.Serialize(recipe.Ingredients) : null,
            Instructions = recipe.Instructions,
            Calories = recipe.Calories,
            Protein = recipe.Protein,
            Carbohydrates = recipe.Carbohydrates,
            Fats = recipe.Fats,
            DietaryLabels = recipe.DietaryLabels,
            CreatedBy = recipe.CreatedBy,
            CreatedAt = DateTime.UtcNow,
        };

        try
        {
            await _db.Recipes.AddAsync(newRecipe);
            await _db.SaveChangesAsync();

            return new RecipesGet
            {
                Id = newRecipe.Id,
                Name = newRecipe.Name,
                Description = newRecipe.Description,
                Ingredients = newRecipe.Ingredients,
                Instructions = newRecipe.Instructions,
                Calories = newRecipe.Calories,
                Protein = newRecipe.Protein,
                Carbohydrates = newRecipe.Carbohydrates,
                Fats = newRecipe.Fats,
                DietaryLabels = newRecipe.DietaryLabels,
                CreatedBy = newRecipe.CreatedBy,
                CreatedAt = newRecipe.CreatedAt,
            };
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"Database error while saving recipe: {ex.InnerException?.Message ?? ex.Message}"
            );
        }
    }

    public async Task<bool> UpdateRecipeAsync(RecipePutRequest recipe)
    {
        var existingRecipe = await _db.Recipes.FindAsync(recipe.Id);
        if (existingRecipe == null)
        {
            return false;
        }

        existingRecipe.Name = recipe.Name;
        existingRecipe.Description = recipe.Description;
        existingRecipe.Ingredients =
            recipe.Ingredients != null ? JsonSerializer.Serialize(recipe.Ingredients) : null;
        existingRecipe.Instructions = recipe.Instructions;
        existingRecipe.Calories = recipe.Calories;
        existingRecipe.Protein = recipe.Protein;
        existingRecipe.Carbohydrates = recipe.Carbohydrates;
        existingRecipe.Fats = recipe.Fats;
        existingRecipe.DietaryLabels = recipe.DietaryLabels;

        try
        {
            _db.Recipes.Update(existingRecipe);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"Database error while updating recipe: {ex.InnerException?.Message ?? ex.Message}"
            );
        }
    }

    public async Task<bool> DeleteRecipeAsync(Guid recipeId)
    {
        var recipeToDelete = await _db.Recipes.FindAsync(recipeId);
        if (recipeToDelete == null)
        {
            return false;
        }

        _db.Recipes.Remove(recipeToDelete);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<RecipesGet>> GetFilteredRecipesAsync(RecipeFilterRequest filter)
    {
        var query = _db.Recipes.AsQueryable();

        if (!string.IsNullOrEmpty(filter.NameSearchTerm))
        {
            query = query.Where(r => r.Name.Contains(filter.NameSearchTerm));
        }

        if (filter.MaxCalories.HasValue)
        {
            query = query.Where(r => r.Calories <= filter.MaxCalories);
        }

        if (filter.MinProtein.HasValue)
        {
            query = query.Where(r => r.Protein >= filter.MinProtein);
        }

        if (filter.MaxCarbohydrates.HasValue)
        {
            query = query.Where(r => r.Carbohydrates <= filter.MaxCarbohydrates);
        }

        if (filter.MaxFats.HasValue)
        {
            query = query.Where(r => r.Fats <= filter.MaxFats);
        }

        return await query
            .Select(r => new RecipesGet
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Ingredients = r.Ingredients,
                Instructions = r.Instructions,
                Calories = r.Calories,
                Protein = r.Protein,
                Carbohydrates = r.Carbohydrates,
                Fats = r.Fats,
                DietaryLabels = r.DietaryLabels,
                CreatedBy = r.CreatedBy,
                CreatedAt = r.CreatedAt,
                Category = r.Category,
                Difficulty = r.Difficulty,
                PrepTime = r.PrepTime,
                CookingTime = r.CookingTime,
                TotalTime = r.TotalTime,
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<RecipesGet>> SearchRecipesByNameAsync(string searchTerm)
    {
        return await _db
            .Recipes.Where(r => r.Name != null && EF.Functions.ILike(r.Name, $"%{searchTerm}%"))
            .Select(r => new RecipesGet
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Ingredients = r.Ingredients,
                Instructions = r.Instructions,
                Calories = r.Calories,
                Protein = r.Protein,
                Carbohydrates = r.Carbohydrates,
                Fats = r.Fats,
                DietaryLabels = r.DietaryLabels,
                CreatedBy = r.CreatedBy,
                CreatedAt = r.CreatedAt,
                Category = r.Category,
                Difficulty = r.Difficulty,
                PrepTime = r.PrepTime,
                CookingTime = r.CookingTime,
                TotalTime = r.TotalTime,
            })
            .ToListAsync();
    }

    public async Task<RecipeReviewResponse> AddRecipeReviewAsync(RecipeReview review)
    {
        await _db.RecipeReviews.AddAsync(review);
        await _db.SaveChangesAsync();

        return new RecipeReviewResponse
        {
            Id = review.Id,
            RecipeId = review.RecipeId,
            UserId = review.UserId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt,
        };
    }

    public async Task<IEnumerable<RecipeReviewResponse>> GetRecipeReviewsAsync(Guid recipeId)
    {
        return await _db
            .RecipeReviews.Include(r => r.User)
            .Where(r => r.RecipeId == recipeId)
            .Select(r => new RecipeReviewResponse
            {
                Id = r.Id,
                RecipeId = r.RecipeId,
                UserId = r.UserId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                UserName = r.User.Username,
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateRecipeReviewAsync(Guid reviewId, RecipeReviewRequest review)
    {
        var existingReview = await _db.RecipeReviews.FindAsync(reviewId);
        if (existingReview == null)
        {
            return false;
        }

        existingReview.Rating = review.Rating;
        existingReview.Comment = review.Comment;
        existingReview.UpdatedAt = DateTime.UtcNow;

        _db.RecipeReviews.Update(existingReview);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRecipeReviewAsync(Guid reviewId)
    {
        var reviewToDelete = await _db.RecipeReviews.FindAsync(reviewId);
        if (reviewToDelete == null)
        {
            return false;
        }

        _db.RecipeReviews.Remove(reviewToDelete);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<RecipeReview?> GetRecipeReviewByIdAsync(Guid reviewId)
    {
        return await _db.RecipeReviews.FindAsync(reviewId);
    }
}
