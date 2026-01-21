using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChinaTown.Application.Dto.Recipe;
using ChinaTown.Application.Services;
using ChinaTown.Domain.Enums;
using ChinaTown.Domain.Exceptions;
using ChinaTown.Web.Extensions;

namespace ChinaTown.Web.Controllers;

[ApiController]
[Route("api/recipes")]
public class RecipeController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRecipes([FromQuery] RecipeFilterDto filter)
    {
        var result = await _recipeService.GetRecipesAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipe(Guid id)
    {
        var recipe = await _recipeService.GetRecipeAsync(id);
        return Ok(recipe);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateRecipe([FromBody] RecipeCreateDto dto)
    {
        var userId = ControllerHelper.GetUserIdFromPrincipals(User);
        var recipe = await _recipeService.CreateRecipeAsync(dto, userId);
        return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] RecipeUpdateDto dto)
    {
        var userId = ControllerHelper.GetUserIdFromPrincipals(User);
        var recipe = await _recipeService.UpdateRecipeAsync(id, dto, userId);
        return Ok(recipe);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(Guid id)
    {
        var userId = ControllerHelper.GetUserIdFromPrincipals(User);
        await _recipeService.DeleteRecipeAsync(id, userId);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadRecipeImage(Guid id, IFormFile file)
    {
        var fileId = Guid.NewGuid();
        using var stream = file.OpenReadStream();
        await _recipeService.UploadImageAsync(id, fileId, file.FileName, stream);
        return Ok(new { fileId });
    }

    [HttpGet("{id}/comments")]
    public async Task<IActionResult> GetRecipeComments(Guid id)
    {
        var result = await _recipeService.GetCommentsAsync(id);
        return Ok(result);
    }

    [HttpGet("{id}/likes")]
    public async Task<IActionResult> GetRecipeLikes(Guid id)
    {
        var result = await _recipeService.GetLikesAsync(id);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyRecipes()
    {
        var userId = ControllerHelper.GetUserIdFromPrincipals(User);
        var result = await _recipeService.GetMyRecipesAsync(userId);
        return Ok(result);
    }
    

    [HttpPatch("{id:guid}/status")]
    [Authorize]
    public async Task<ActionResult> ChangeStatus(Guid id, [FromBody] string status)
    {
        var currentUserId = ControllerHelper.GetUserIdFromPrincipals(User);

        if (status != nameof(ContentStatus.Archived) &&
            status != nameof(ContentStatus.Draft) &&
            status != nameof(ContentStatus.Published))
        {
            return BadRequest(new {message = "Invalid status"});
        }
        
        var contentStatus = status == nameof(ContentStatus.Archived) ? ContentStatus.Archived 
            : status == nameof(ContentStatus.Draft) ? ContentStatus.Draft 
            : ContentStatus.Archived;
        
        await _recipeService.ChangeStatusAsync(id, currentUserId, contentStatus);
        
        return Ok(new { message = "Recipe status changed successfully" });
    }

    private string GetContentTypeFromFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}