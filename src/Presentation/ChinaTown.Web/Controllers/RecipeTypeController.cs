using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChinaTown.Application.Dto.RecipeType;
using ChinaTown.Application.Services;

namespace ChinaTown.Web.Controllers;

[ApiController]
[Route("api/recipe-types")]
public class RecipeTypeController : ControllerBase
{
    private readonly IRecipeTypeService _recipeTypeService;

    public RecipeTypeController(IRecipeTypeService recipeTypeService)
    {
        _recipeTypeService = recipeTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _recipeTypeService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var recipeType = await _recipeTypeService.GetByIdAsync(id);
        return Ok(recipeType);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] RecipeTypeCreateDto dto)
    {
        var recipeType = await _recipeTypeService.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = recipeType.Id }, recipeType);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] RecipeTypeUpdateDto dto)
    {
        var recipeType = await _recipeTypeService.UpdateAsync(id, dto);
        return Ok(recipeType);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _recipeTypeService.DeleteAsync(id);
        return NoContent();
    }
}