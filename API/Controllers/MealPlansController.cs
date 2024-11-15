using API.BLL.Services.Abstraction;
using API.Requests.MealPlans;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MealPlansController : ControllerBase
{
    private readonly IMealPlanGeneratorService _mealPlanGeneratorService;

    public MealPlansController(IMealPlanGeneratorService mealPlanGeneratorService)
    {
        _mealPlanGeneratorService = mealPlanGeneratorService;
    }

    [HttpPost("generate")]
    public async Task<ActionResult<MealPlanResponse>> GenerateMealPlan(
        [FromBody] MealPlanGenerateRequest request
    )
    {
        try
        {
            var mealPlan = await _mealPlanGeneratorService.GenerateMealPlanAsync(request);
            return Ok(mealPlan);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                $"An error occurred while generating the meal plan: {ex.Message}"
            );
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MealPlanResponse>> GetMealPlan(Guid id)
    {
        var mealPlan = await _mealPlanGeneratorService.GetMealPlanByIdAsync(id);
        if (mealPlan == null)
            return NotFound();
        return Ok(mealPlan);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<MealPlanResponse>>> GetUserMealPlans(Guid userId)
    {
        var mealPlans = await _mealPlanGeneratorService.GetUserMealPlansAsync(userId);
        return Ok(mealPlans);
    }

    [HttpGet("{userId}/current")]
    public async Task<ActionResult<MealPlanResponse>> GetCurrentMealPlan(Guid userId)
    {
        try
        {
            var mealPlan = await _mealPlanGeneratorService.GetCurrentMealPlanAsync(userId);
            if (mealPlan == null)
                return NotFound("No active meal plan found for the user");

            return Ok(mealPlan);
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to get current meal plan: {ex.Message}");
        }
    }
}
