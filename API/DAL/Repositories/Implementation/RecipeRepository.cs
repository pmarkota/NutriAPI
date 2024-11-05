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
        return await _db.Recipes.Select(r => new RecipesGet
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
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<RecipesGet?> GetRecipeByIdAsync(Guid recipeId)
    {
        return await _db.Recipes.Where(r => r.Id == recipeId)
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
                CreatedAt = r.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddRecipeAsync(RecipePostRequest recipe)
    {
        var newRecipe = new Recipe
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            Ingredients = recipe.Ingredients,
            Instructions = recipe.Instructions,
            Calories = recipe.Calories,
            Protein = recipe.Protein,
            Carbohydrates = recipe.Carbohydrates,
            Fats = recipe.Fats,
            DietaryLabels = recipe.DietaryLabels,
            CreatedBy = recipe.CreatedBy,
            CreatedAt = DateTime.Now
        };

        _db.Recipes.Add(newRecipe);
        await _db.SaveChangesAsync();
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
        existingRecipe.Ingredients = recipe.Ingredients;
        existingRecipe.Instructions = recipe.Instructions;
        existingRecipe.Calories = recipe.Calories;
        existingRecipe.Protein = recipe.Protein;
        existingRecipe.Carbohydrates = recipe.Carbohydrates;
        existingRecipe.Fats = recipe.Fats;
        existingRecipe.DietaryLabels = recipe.DietaryLabels;
        existingRecipe.CreatedBy = recipe.CreatedBy;

        _db.Recipes.Update(existingRecipe);
        await _db.SaveChangesAsync();
        return true;
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
}