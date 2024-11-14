using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using YourNamespace.Models;
using YourNamespace.Models.Requests;
using YourNamespace.Services;

namespace YourNamespace.Services
{
    public class RecipeService
    {
        private readonly YourNamespaceContext _context;

        public RecipeService(YourNamespaceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Recipe>> FilterRecipesAsync(RecipeFilterRequest filter)
        {
            var query = _context.Recipes
                .Include(r => r.User)
                .Select(r => new Recipe
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
                    User = r.User
                })
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(r => r.Name.Contains(filter.SearchTerm) || 
                                        r.Description.Contains(filter.SearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(filter.Category))
            {
                query = query.Where(r => r.Category == filter.Category);
            }

            if (!string.IsNullOrWhiteSpace(filter.Difficulty))
            {
                query = query.Where(r => r.Difficulty == filter.Difficulty);
            }

            if (filter.MaxPrepTime.HasValue)
            {
                query = query.Where(r => r.PrepTime <= filter.MaxPrepTime);
            }

            if (filter.MaxCookingTime.HasValue)
            {
                query = query.Where(r => r.CookingTime <= filter.MaxCookingTime);
            }

            if (filter.MaxTotalTime.HasValue)
            {
                query = query.Where(r => r.TotalTime <= filter.MaxTotalTime);
            }

            if (filter.MaxCalories.HasValue)
            {
                query = query.Where(r => r.Calories <= filter.MaxCalories);
            }

            if (!string.IsNullOrWhiteSpace(filter.DietaryLabels))
            {
                query = query.Where(r => r.DietaryLabels.Contains(filter.DietaryLabels));
            }

            return await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        public async Task<Recipe> CreateRecipeAsync(RecipePostRequest request, Guid userId)
        {
            var recipe = new Recipe
            {
                Name = request.Name,
                Description = request.Description,
                Ingredients = request.Ingredients,
                Instructions = request.Instructions,
                Calories = request.Calories,
                Protein = request.Protein,
                Carbohydrates = request.Carbohydrates,
                Fats = request.Fats,
                DietaryLabels = request.DietaryLabels,
                Category = request.Category,
                Difficulty = request.Difficulty,
                PrepTime = request.PrepTime,
                CookingTime = request.CookingTime,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task<Recipe> UpdateRecipeAsync(RecipePutRequest request)
        {
            var recipe = await _context.Recipes.FindAsync(request.Id);
            if (recipe == null)
            {
                throw new NotFoundException("Recipe not found");
            }

            recipe.Name = request.Name;
            recipe.Description = request.Description;
            recipe.Ingredients = request.Ingredients;
            recipe.Instructions = request.Instructions;
            recipe.Calories = request.Calories;
            recipe.Protein = request.Protein;
            recipe.Carbohydrates = request.Carbohydrates;
            recipe.Fats = request.Fats;
            recipe.DietaryLabels = request.DietaryLabels;
            recipe.Category = request.Category;
            recipe.Difficulty = request.Difficulty;
            recipe.PrepTime = request.PrepTime;
            recipe.CookingTime = request.CookingTime;

            await _context.SaveChangesAsync();
            return recipe;
        }
    }
} 