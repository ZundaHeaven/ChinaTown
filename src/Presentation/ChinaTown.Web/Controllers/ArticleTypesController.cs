// ChinaTown.Web/Controllers/ArticleTypesController.cs
using ChinaTown.Application.Dto.Article;
using ChinaTown.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChinaTown.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ArticleTypesController : ControllerBase
{
    private readonly IArticleTypeService _articleTypeService;

    public ArticleTypesController(IArticleTypeService articleTypeService)
    {
        _articleTypeService = articleTypeService;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ArticleTypeDto>>> GetArticleTypes()
    {
        var types = await _articleTypeService.GetAllArticleTypesAsync();
        return Ok(types);
    }
    
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ArticleTypeDto>> GetArticleType(Guid id)
    {
        var type = await _articleTypeService.GetArticleTypeByIdAsync(id);
        
        if (type == null)
            return NotFound();
        
        return Ok(type);
    }
    
    [HttpPost]
    public async Task<ActionResult<ArticleTypeDto>> CreateArticleType(ArticleTypeCreateDto dto)
    {
        var type = await _articleTypeService.CreateArticleTypeAsync(dto);
        return CreatedAtAction(nameof(GetArticleType), new { id = type.Id }, type);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ArticleTypeDto>> UpdateArticleType(Guid id, ArticleTypeUpdateDto dto)
    {
        var type = await _articleTypeService.UpdateArticleTypeAsync(id, dto);
        return Ok(type);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteArticleType(Guid id)
    {
        await _articleTypeService.DeleteArticleTypeAsync(id);
        return NoContent();
    }
}