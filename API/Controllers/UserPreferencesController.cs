using API.BLL.Services.Abstraction;
using API.DAL.Models;
using API.Requests.UserPreferences;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserPreferencesController : ControllerBase
{
    private readonly IUserPreferencesService _userPreferencesService;

    public UserPreferencesController(IUserPreferencesService userPreferencesService)
    {
        _userPreferencesService = userPreferencesService;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserPreference>> GetUserPreferences(Guid userId)
    {
        var preferences = await _userPreferencesService.GetUserPreferencesAsync(userId);
        if (preferences == null)
            return NotFound();

        return Ok(preferences);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserPreferences(
        [FromBody] UpdateUserPreferencesRequest request
    )
    {
        var success = await _userPreferencesService.UpdateUserPreferencesAsync(request);
        if (!success)
            return NotFound();

        return NoContent();
    }

    /*[HttpPost]
    public async Task<IActionResult> CreateUserPreferences(
        [FromBody] CreateUserPreferencesRequest request
    )
    {
        var success = await _userPreferencesService.CreateUserPreferencesAsync(request);
        if (!success)
            return BadRequest();

        return CreatedAtAction(
            nameof(GetUserPreferences),
            new { userId = request.UserId },
            request
        );
    }*/

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUserPreferences(Guid userId)
    {
        var success = await _userPreferencesService.DeleteUserPreferencesAsync(userId);
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{userId}/favorite-recipes")]
    public async Task<IActionResult> AddFavoriteRecipe(
        Guid userId,
        [FromBody] AddFavoriteRecipeRequest request
    )
    {
        var success = await _userPreferencesService.AddFavoriteRecipeAsync(
            userId,
            request.RecipeId
        );
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{userId}/favorite-recipes/{recipeId}")]
    public async Task<IActionResult> RemoveFavoriteRecipe(Guid userId, Guid recipeId)
    {
        var success = await _userPreferencesService.RemoveFavoriteRecipeAsync(userId, recipeId);
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{userId}/excluded-ingredients")]
    public async Task<IActionResult> AddExcludedIngredient(
        Guid userId,
        [FromBody] AddExcludedIngredientRequest request
    )
    {
        var success = await _userPreferencesService.AddExcludedIngredientAsync(
            userId,
            request.Ingredient
        );
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{userId}/excluded-ingredients/{ingredient}")]
    public async Task<IActionResult> RemoveExcludedIngredient(Guid userId, string ingredient)
    {
        var success = await _userPreferencesService.RemoveExcludedIngredientAsync(
            userId,
            ingredient
        );
        if (!success)
            return NotFound();

        return NoContent();
    }
}
