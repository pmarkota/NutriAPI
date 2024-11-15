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
            if (recipe == null)
                return NotFound();
            return Ok(recipe);
        }

        [HttpPost]
        public async Task<ActionResult> CreateRecipe(RecipePostRequest recipe)
        {
            try
            {
                var createdRecipe = await _recipeService.AddRecipeAsync(recipe);
                return CreatedAtAction(
                    nameof(GetRecipeById),
                    new { id = createdRecipe.Id },
                    createdRecipe
                );
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create recipe: {ex.Message}");
            }
        }

        [HttpPost("update")]
        public async Task<ActionResult> UpdateRecipe([FromBody] RecipePutRequest recipe)
        {
            var existingRecipe = await _recipeService.GetRecipeByIdAsync(recipe.Id);
            if (existingRecipe == null)
            {
                return NotFound();
            }

            try
            {
                var updated = await _recipeService.UpdateRecipeAsync(recipe);
                if (!updated)
                {
                    return BadRequest("The update operation failed.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update recipe: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRecipe(Guid id)
        {
            var deleted = await _recipeService.DeleteRecipeAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }

        [HttpPost("filter")]
        public async Task<ActionResult<IEnumerable<RecipesGet>>> FilterRecipes(
            [FromBody] RecipeFilterRequest filter
        )
        {
            try
            {
                if (!string.IsNullOrEmpty(filter.NameSearchTerm) && filter.NameSearchTerm.Length < 2)
                {
                    return BadRequest("Search term must be at least 2 characters long");
                }

                var recipes = await _recipeService.GetFilteredRecipesAsync(filter);
                return Ok(recipes);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to filter recipes: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<RecipesGet>>> SearchRecipes(
            [FromQuery] string searchTerm
        )
        {
            try
            {
                var recipes = await _recipeService.SearchRecipesByNameAsync(searchTerm);
                return Ok(recipes);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to search recipes: {ex.Message}");
            }
        }
    }
}
