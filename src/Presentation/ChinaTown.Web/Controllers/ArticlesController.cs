using ChinaTown.Application.Dto.Article;
using ChinaTown.Application.Services;
using ChinaTown.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public async Task<ActionResult<ArticleDto>> GetArticle(Guid id)
    {
        var article = await _articleService.GetArticleByIdAsync(id);
        
        if (article == null)
            return NotFound();
            
        var currentUserId = ControllerHelper.GetUserIdFromPrincipals(User);
        var isAdmin = User.IsInRole("Admin");
        
        if (article.Status != "Published" && article.AuthorId != currentUserId && !isAdmin)
        {
            return NotFound();
        }
        
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

    [HttpGet("admin/all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetAllArticlesAdmin(
        [FromQuery] string? status = null,
        [FromQuery] Guid? authorId = null,
        [FromQuery] string? search = null)
    {
        var filter = new ArticleFilterDto
        {
            Status = status,
            AuthorId = authorId,
            Search = search,
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

    [HttpPost("{id:guid}/publish")]
    [Authorize]
    public async Task<ActionResult> PublishArticle(Guid id)
    {
        var currentUserId = ControllerHelper.GetUserIdFromPrincipals(User);
        var updateDto = new ArticleUpdateDto
        {
            Status = "Published"
        };
        
        await _articleService.UpdateArticleAsync(id, updateDto, currentUserId);
        
        return Ok(new { message = "Article published successfully" });
    }

    [HttpPost("{id:guid}/unpublish")]
    [Authorize]
    public async Task<ActionResult> UnpublishArticle(Guid id)
    {
        var currentUserId = ControllerHelper.GetUserIdFromPrincipals(User);
        
        var updateDto = new ArticleUpdateDto
        {
            Status = "Draft"
        };
        
        await _articleService.UpdateArticleAsync(id, updateDto, currentUserId);
        
        return Ok(new { message = "Article unpublished successfully" });
    }

    [HttpPost("{id:guid}/archive")]
    [Authorize]
    public async Task<ActionResult> ArchiveArticle(Guid id)
    {
        var currentUserId = ControllerHelper.GetUserIdFromPrincipals(User);
        var updateDto = new ArticleUpdateDto
        {
            Status = "Archived"
        };
        
        await _articleService.UpdateArticleAsync(id, updateDto, currentUserId);
        
        return Ok(new { message = "Article archived successfully" });
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

    [HttpPost("{id:guid}/restore")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RestoreArticle(Guid id)
    {
        var article = await _articleService.GetArticleByIdAsync(id);
        if (article == null)
            return NotFound();
        
        var updateDto = new ArticleUpdateDto
        {
            Status = "Draft"
        };
        
        var currentUserId = ControllerHelper.GetUserIdFromPrincipals(User);
        var updatedArticle = await _articleService.UpdateArticleAsync(id, updateDto, currentUserId);
        
        return Ok(updatedArticle);
    }
}