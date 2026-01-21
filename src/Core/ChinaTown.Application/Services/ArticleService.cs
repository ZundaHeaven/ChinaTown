using AutoMapper;
using ChinaTown.Application.Data;
using ChinaTown.Application.Dto.Article;
using ChinaTown.Application.Dto.Common;
using ChinaTown.Application.Helpers;
using ChinaTown.Domain.Entities;
using ChinaTown.Domain.Enums;
using ChinaTown.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChinaTown.Application.Services;

public class ArticleService : IArticleService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ArticleService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ArticleDto>> GetArticlesAsync(ArticleFilterDto filter)
    {
        var query = BuildArticleQuery(filter);

        var articles = await query.ToListAsync();

        var articleDtos = _mapper.Map<IEnumerable<ArticleDto>>(articles);

        return articleDtos;
    }

    public async Task<ArticleDto?> GetArticleByIdAsync(Guid id)
    {
        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleType)
            .Include(a => a.Likes)
            .Include(a => a.Comments)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (article == null)
            return null;

        return _mapper.Map<ArticleDto>(article);
    }

    public async Task<ArticleDto?> GetPublishedArticleByIdAsync(Guid id)
    {
        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleType)
            .Include(a => a.Likes)
            .Include(a => a.Comments)
            .FirstOrDefaultAsync(a => a.Id == id && a.Status == ContentStatus.Published);

        if (article == null)
            return null;

        return _mapper.Map<ArticleDto>(article);
    }

    public async Task<ArticleDto> CreateArticleAsync(ArticleCreateDto dto, Guid authorId)
    {
        var articleType = await _context.ArticleTypes
            .FirstOrDefaultAsync(at => at.Id == dto.ArticleTypeId);
        
        if (articleType == null)
            throw new NotFoundException($"Article type with id {dto.ArticleTypeId} not found");

        var author = await _context.Users.FindAsync(authorId);
        if (author == null)
            throw new NotFoundException($"User with id {authorId} not found");

        var slug = SlugHelper.GenerateSlug("article", dto.Title);

        var existingArticle = await _context.Content
            .FirstOrDefaultAsync(c => c.Slug == slug);

        if (existingArticle != null)
            slug = SlugHelper.GenerateSlug("article", "");

        var article = new Article
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Slug = slug,
            Excerpt = dto.Excerpt ?? string.Empty,
            Body = dto.Body,
            ReadingTimeMinutes = dto.ReadingTimeMinutes,
            ArticleTypeId = dto.ArticleTypeId,
            UserId = authorId,
            Status = ContentStatus.Draft
        };

        _context.Articles.Add(article);
        await _context.SaveChangesAsync();

        var createdArticle = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleType)
            .FirstOrDefaultAsync(a => a.Id == article.Id);

        if (createdArticle == null)
            throw new Exception("Failed to create article");

        return _mapper.Map<ArticleDto>(createdArticle);
    }

    public async Task<ArticleDto> UpdateArticleAsync(Guid id, ArticleUpdateDto dto, Guid currentUserId)
    {
        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleType)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (article == null)
            throw new NotFoundException($"Article with id {id} not found");

        if (article.UserId != currentUserId)
        {
            var currentUser = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);
            
            if (currentUser?.Role.ToString() != "Admin")
                throw new ForbiddenException("You don't have permission to update this article");
        }

        if (!string.IsNullOrWhiteSpace(dto.Title))
        {
            article.Title = dto.Title;
            var slug = SlugHelper.GenerateSlug("Article", dto.Title);
        
            var extistedArticle = _context.Content.FirstOrDefault(b => b.Slug == slug);

            if (extistedArticle != null)
                slug = SlugHelper.GenerateSlug("Article", "");


            article.Slug = slug;
        }

        if (!string.IsNullOrWhiteSpace(dto.Body))
            article.Body = dto.Body;

        if (!string.IsNullOrWhiteSpace(dto.Excerpt))
            article.Excerpt = dto.Excerpt;

        if (dto.ArticleTypeId.HasValue)
        {
            var articleType = await _context.ArticleTypes
                .FirstOrDefaultAsync(at => at.Id == dto.ArticleTypeId.Value);
            
            if (articleType == null)
                throw new NotFoundException($"Article type with id {dto.ArticleTypeId} not found");
            
            article.ArticleTypeId = dto.ArticleTypeId.Value;
        }

        if (dto.ReadingTimeMinutes.HasValue)
            article.ReadingTimeMinutes = dto.ReadingTimeMinutes.Value;

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            if (Enum.TryParse<ContentStatus>(dto.Status, out var status))
                article.Status = status;
        }

        await _context.SaveChangesAsync();

        await _context.Entry(article)
            .Reference(a => a.ArticleType)
            .LoadAsync();
        
        await _context.Entry(article)
            .Reference(a => a.Author)
            .LoadAsync();

        return _mapper.Map<ArticleDto>(article);
    }

    public async Task DeleteArticleAsync(Guid id, Guid currentUserId)
    {
        var article = await _context.Articles
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (article == null)
            throw new NotFoundException($"Article with id {id} not found");

        if (article.UserId != currentUserId)
        {
            var currentUser = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);
            
            if (currentUser?.Role.ToString() != "Admin")
                throw new ForbiddenException("You don't have permission to delete this article");
        }

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CommentDto>> GetArticleCommentsAsync(Guid articleId)
    {
        var article = await _context.Articles.FindAsync(articleId);
        if (article == null)
            throw new NotFoundException($"Article with id {articleId} not found");

        var query = _context.Comments
            .Include(c => c.User)
            .OrderByDescending(c => c.CreatedOn);

        var totalCount = await query.CountAsync();

        var comments = await query
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }
    
    public async Task<IEnumerable<LikeDto>> GetArticleLikesAsync(Guid articleId)
    {
        var article = await _context.Articles.FindAsync(articleId);
        if (article == null)
            throw new NotFoundException($"Article with id {articleId} not found");

        var query = _context.Likes
            .Include(l => l.User)
            .Where(l => l.ContentId == articleId)
            .OrderByDescending(l => l.CreatedOn);

        var totalCount = await query.CountAsync();

        var likes = await query
            .ToListAsync();

        return _mapper.Map<IEnumerable<LikeDto>>(likes);
    }

    private IQueryable<Article> BuildArticleQuery(ArticleFilterDto filter)
    {
        var query = _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleType)
            .Include(a => a.Likes)
            .Include(a => a.Comments)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            if (Enum.TryParse<ContentStatus>(filter.Status, true, out var status))
            {
                query = query.Where(a => a.Status == status);
            }
        }

        if (!string.IsNullOrWhiteSpace(filter.Type))
        {
            query = query.Where(a => a.ArticleType.Name == filter.Type);
        }

        if (filter.AuthorId.HasValue)
        {
            query = query.Where(a => a.UserId == filter.AuthorId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchTerm = filter.Search.ToLower();
            query = query.Where(a => 
                a.Title.ToLower().Contains(searchTerm) ||
                a.Body.ToLower().Contains(searchTerm) ||
                a.Excerpt.ToLower().Contains(searchTerm));
        }

        query = filter.Sort?.ToLower() switch
        {
            "oldest" => query.OrderBy(a => a.CreatedOn),
            "most_liked" => query.OrderByDescending(a => a.Likes.Count),
            "most_commented" => query.OrderByDescending(a => a.Comments.Count),
            _ => query.OrderByDescending(a => a.CreatedOn) 
        };

        return query;
    }

    public async Task<bool> ChangeStatusAsync(Guid articleId, Guid currentUserId, ContentStatus status)
    {
        var article = await _context.Articles.FindAsync(articleId);
        if (article == null)
            throw new NotFoundException($"Article with id {articleId} not found");

        if (article.UserId != currentUserId)
        {
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == currentUserId);
            
            if (currentUser?.Role.ToString() != "Admin")
                throw new ForbiddenException("You don't have permission to change status for this article");
        }

        article.Status = status;
        article.ModifiedOn = DateTime.UtcNow;
        
        _context.Update(article);
        

        await _context.SaveChangesAsync();
        return true;
    }
}