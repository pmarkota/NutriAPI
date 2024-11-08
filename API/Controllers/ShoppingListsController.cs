using API.BLL.Services.Abstraction;
using API.Requests.ShoppingLists;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShoppingListsController : ControllerBase
{
    private readonly IShoppingListService _shoppingListService;

    public ShoppingListsController(IShoppingListService shoppingListService)
    {
        _shoppingListService = shoppingListService;
    }

    [HttpPost("generate")]
    public async Task<ActionResult<ShoppingListResponse>> GenerateShoppingList(
        [FromBody] GenerateShoppingListRequest request
    )
    {
        try
        {
            var shoppingList = await _shoppingListService.GenerateFromMealPlanAsync(
                request.UserId,
                request.MealPlanId
            );
            return Ok(shoppingList);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShoppingListResponse>> GetShoppingList(Guid id)
    {
        var shoppingList = await _shoppingListService.GetByIdAsync(id);
        if (shoppingList == null)
            return NotFound();
        return Ok(shoppingList);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<ShoppingListResponse>>> GetUserShoppingLists(
        Guid userId
    )
    {
        var shoppingLists = await _shoppingListService.GetUserShoppingListsAsync(userId);
        return Ok(shoppingLists);
    }

    [HttpPut("items/{itemId}/toggle")]
    public async Task<ActionResult> ToggleShoppingListItem(Guid itemId, [FromBody] bool isChecked)
    {
        var success = await _shoppingListService.ToggleItemCheckAsync(itemId, isChecked);
        if (!success)
            return NotFound();
        return NoContent();
    }
}
