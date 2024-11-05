using API.BLL.Services.Abstraction;
using API.Requests.Recipes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipesGet>>> GetAllRecipes()
        {
            var recipes = await _recipeService.GetAllRecipesAsync();
            return Ok(recipes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecipesGet>> GetRecipeById(Guid id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            if (recipe == null) return NotFound();
            return Ok(recipe);
        }

        [HttpPost]
        public async Task<ActionResult> CreateRecipe(RecipePostRequest recipe)
        {
            await _recipeService.AddRecipeAsync(recipe);
            return CreatedAtAction(nameof(GetRecipeById), new { id = recipe.Id }, recipe);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRecipe(RecipePutRequest recipe)
        {
            var existingRecipe = await _recipeService.GetRecipeByIdAsync(recipe.Id);
            if (existingRecipe == null) return NotFound();

            var updated = await _recipeService.UpdateRecipeAsync(recipe);
            if (!updated) return BadRequest("The update operation failed.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRecipe(Guid id)
        {
            var deleted = await _recipeService.DeleteRecipeAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}