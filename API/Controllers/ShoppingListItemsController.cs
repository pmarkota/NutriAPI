using API.BLL.Services.Abstraction;
using API.Requests.ShoppingLists;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShoppingListItemsController : ControllerBase
{
    private readonly IShoppingListItemService _service;

    public ShoppingListItemsController(IShoppingListItemService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShoppingListItemResponse>> GetById(Guid id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null)
            return NotFound();
        return Ok(item);
    }

    [HttpGet("list/{shoppingListId}")]
    public async Task<ActionResult<IEnumerable<ShoppingListItemResponse>>> GetByShoppingListId(
        Guid shoppingListId
    )
    {
        var items = await _service.GetByShoppingListIdAsync(shoppingListId);
        return Ok(items);
    }

    [HttpPut("{id}/toggle")]
    public async Task<ActionResult> ToggleItem(Guid id, [FromBody] bool isChecked)
    {
        var success = await _service.ToggleItemAsync(id, isChecked);
        if (!success)
            return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteItem(Guid id)
    {
        var success = await _service.DeleteItemAsync(id);
        if (!success)
            return NotFound();
        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingListItemResponse>> AddItem(
        [FromBody] AddShoppingListItemRequest request
    )
    {
        try
        {
            var item = await _service.AddItemAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
