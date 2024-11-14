using System.Text.Json;
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
                TotalTime = r.TotalTime
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

        System.Diagnostics.Debug.WriteLine($"Initial query count: {await query.CountAsync()}");

        if (!string.IsNullOrEmpty(filter.DietaryPreference))
        {
            var normalizedPreference = filter.DietaryPreference.ToLower().Trim();

            if (normalizedPreference != "all" && normalizedPreference != "any")
            {
                query = query.Where(r =>
                    r.DietaryLabels != null
                    && EF.Functions.ILike(r.DietaryLabels.ToLower(), $"%{normalizedPreference}%")
                );
            }
            System.Diagnostics.Debug.WriteLine(
                $"After dietary filter count: {await query.CountAsync()}"
            );
        }

        if (filter.MaxCalories.HasValue)
        {
            query = query.Where(r => r.Calories.HasValue && r.Calories <= filter.MaxCalories);
            System.Diagnostics.Debug.WriteLine(
                $"After calories filter count: {await query.CountAsync()}"
            );
        }

        if (filter.MinProtein.HasValue)
        {
            query = query.Where(r => r.Protein.HasValue && r.Protein >= filter.MinProtein);
            System.Diagnostics.Debug.WriteLine(
                $"After protein filter count: {await query.CountAsync()}"
            );
        }

        if (filter.MaxCarbohydrates.HasValue)
        {
            query = query.Where(r =>
                r.Carbohydrates.HasValue && r.Carbohydrates <= filter.MaxCarbohydrates
            );
            System.Diagnostics.Debug.WriteLine(
                $"After carbs filter count: {await query.CountAsync()}"
            );
        }

        if (filter.MaxFats.HasValue)
        {
            query = query.Where(r => r.Fats.HasValue && r.Fats <= filter.MaxFats);
            System.Diagnostics.Debug.WriteLine(
                $"After fats filter count: {await query.CountAsync()}"
            );
        }

        var results = await query
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
                TotalTime = r.TotalTime
            })
            .ToListAsync();

        // Debug logging for all recipes if no results found
        if (!results.Any())
        {
            var allRecipes = await _db
                .Recipes.Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.DietaryLabels,
                    r.Calories,
                    r.Protein,
                    r.Carbohydrates,
                    r.Fats,
                })
                .ToListAsync();

            System.Diagnostics.Debug.WriteLine("\n=== Filter Criteria ===");
            System.Diagnostics.Debug.WriteLine($"DietaryPreference: {filter.DietaryPreference}");
            System.Diagnostics.Debug.WriteLine($"MaxCalories: {filter.MaxCalories}");
            System.Diagnostics.Debug.WriteLine($"MinProtein: {filter.MinProtein}");
            System.Diagnostics.Debug.WriteLine($"MaxCarbohydrates: {filter.MaxCarbohydrates}");
            System.Diagnostics.Debug.WriteLine($"MaxFats: {filter.MaxFats}");

            System.Diagnostics.Debug.WriteLine("\n=== Available Recipes ===");
            foreach (var recipe in allRecipes)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Recipe: {recipe.Name} (ID: {recipe.Id})\n"
                        + $"Labels: {recipe.DietaryLabels}\n"
                        + $"Calories: {recipe.Calories}, "
                        + $"Protein: {recipe.Protein}, "
                        + $"Carbs: {recipe.Carbohydrates}, "
                        + $"Fats: {recipe.Fats}\n"
                );
            }
        }

        return results;
    }
}
