using ChinaTown.Application.Dto.Article;
using ChinaTown.Application.Services;
using ChinaTown.Domain.Enums;
using ChinaTown.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ChinaTown.Web.Controllers;

[ApiController]
[Route("api/articles")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet]
    public async Task<ActionResult> GetArticles([FromQuery] ArticleFilterDto filter)
    {
        var result = await _articleService.GetArticlesAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ArticleDto>> GetArticle(Guid id)
    {
        var article = await _articleService.GetArticleByIdAsync(id);
        
        if (article == null)
            return NotFound();
        
        return Ok(article);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult> GetMyArticles(
        [FromQuery] string? status = null)
    {
        var currentUserId = ControllerHelper.GetUserIdFromPrincipals(User);
        
        var filter = new ArticleFilterDto
        {
            AuthorId = currentUserId,
            Status = status,
            Sort = "newest"
        };
        
        var result = await _articleService.GetArticlesAsync(filter);
        return Ok(result);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ArticleDto>> CreateArticle(ArticleCreateDto dto)
    {
        var authorId = ControllerHelper.GetUserIdFromPrincipals(User);
        var article = await _articleService.CreateArticleAsync(dto, authorId);
        return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
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
        
        ContentStatus contentStatus = status == nameof(ContentStatus.Archived) ? ContentStatus.Archived 
            : status == nameof(ContentStatus.Draft) ? ContentStatus.Draft 
            : ContentStatus.Archived;
        
        await _articleService.ChangeStatusAsync(id, currentUserId, contentStatus);
        
        return Ok(new { message = "Article changed successfully" });
    }
    
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ArticleDto>> UpdateArticle(Guid id, ArticleUpdateDto dto)
    {
        var currentUserId = ControllerHelper.GetUserIdFromPrincipals(User);
        var article = await _articleService.UpdateArticleAsync(id, dto, currentUserId);
        return Ok(article);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> DeleteArticle(Guid id)
    {
        var currentUserId = ControllerHelper.GetUserIdFromPrincipals(User);
        await _articleService.DeleteArticleAsync(id, currentUserId);
        return NoContent();
    }

    [HttpGet("{id:guid}/comments")]
    public async Task<ActionResult> GetArticleComments(
        Guid id)
    {
        var result = await _articleService.GetArticleCommentsAsync(id);
        return Ok(result);
    }

    [HttpGet("{id:guid}/likes")]
    public async Task<ActionResult> GetArticleLikes(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _articleService.GetArticleLikesAsync(id);
        return Ok(result);
    }

    [HttpGet("by-slug/{slug}")]
    public async Task<ActionResult<ArticleDto>> GetArticleBySlug(string slug)
    {
        var filter = new ArticleFilterDto
        {
            Search = slug,
            Status = "Published"
        };
        
        var result = await _articleService.GetArticlesAsync(filter);
        var article = result.FirstOrDefault(a => a.Slug == slug);
        
        if (article == null)
            return NotFound();
        
        return Ok(article);
    }
}