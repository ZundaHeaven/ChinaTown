using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChinaTown.Application.Dto.Recipe;
using ChinaTown.Application.Services;
using ChinaTown.Domain.Exceptions;

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
        var userId = GetCurrentUserId();
        var recipe = await _recipeService.CreateRecipeAsync(dto, userId);
        return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] RecipeUpdateDto dto)
    {
        var userId = GetCurrentUserId();
        var recipe = await _recipeService.UpdateRecipeAsync(id, dto, userId);
        return Ok(recipe);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(Guid id)
    {
        var userId = GetCurrentUserId();
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
        var userId = GetCurrentUserId();
        var result = await _recipeService.GetMyRecipesAsync(userId);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("archive")]
    public async Task<IActionResult> GetArchivedRecipes()
    {
        var userId = GetCurrentUserId();
        var result = await _recipeService.GetArchivedRecipesAsync(userId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("{id}/publish")]
    public async Task<IActionResult> PublishRecipe(Guid id)
    {
        var userId = GetCurrentUserId();
        await _recipeService.PublishRecipeAsync(id, userId);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/unpublish")]
    public async Task<IActionResult> UnpublishRecipe(Guid id)
    {
        var userId = GetCurrentUserId();
        await _recipeService.UnpublishRecipeAsync(id, userId);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/archive")]
    public async Task<IActionResult> ArchiveRecipe(Guid id)
    {
        var userId = GetCurrentUserId();
        await _recipeService.ArchiveRecipeAsync(id, userId);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> RestoreRecipe(Guid id)
    {
        var userId = GetCurrentUserId();
        await _recipeService.RestoreRecipeAsync(id, userId);
        return NoContent();
    }

    [HttpGet("difficulties")]
    public IActionResult GetDifficulties()
    {
        var difficulties = Enum.GetValues(typeof(ChinaTown.Domain.Enums.RecipeDifficulty))
            .Cast<ChinaTown.Domain.Enums.RecipeDifficulty>()
            .Select(d => new 
            { 
                Id = (int)d, 
                Name = d.ToString() 
            })
            .ToList();

        return Ok(difficulties);
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("User not authenticated");
        return Guid.Parse(userId);
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