using ChinaTown.Application.Data;
using ChinaTown.Application.Dto.Article;
using ChinaTown.Application.Dto.Common;
using ChinaTown.Domain.Entities;
using ChinaTown.Domain.Enums;
using ChinaTown.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChinaTown.Application.Services;

public class ArticleService : IArticleService
{
    private readonly ApplicationDbContext _context;

    public ArticleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<ArticleDto>> GetArticlesAsync(ArticleFilterDto filter)
    {
        var query = BuildArticleQuery(filter);
        var totalCount = await query.CountAsync();

        var articles = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var articleDtos = articles.Select(MapToDto).ToList();

        return new PaginatedResult<ArticleDto>
        {
            Items = articleDtos,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ArticleDto?> GetArticleByIdAsync(Guid id)
    {
        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleType)
            .Include(a => a.Likes)
            .Include(a => a.Comments)
            .Include(a => a.Views)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (article == null)
            return null;

        return MapToDto(article);
    }

    public async Task<ArticleDto?> GetPublishedArticleByIdAsync(Guid id)
    {
        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleType)
            .Include(a => a.Likes)
            .Include(a => a.Comments)
            .Include(a => a.Views)
            .FirstOrDefaultAsync(a => a.Id == id && a.Status == ContentStatus.Published);

        if (article == null)
            return null;

        return MapToDto(article);
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

        var slug = GenerateSlug(dto.Title);

        var existingArticle = await _context.Articles
            .FirstOrDefaultAsync(c => c.Slug == slug);
        
        if (existingArticle != null)
            slug = $"{slug}-{DateTime.UtcNow.Ticks}";

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

        // Получаем созданную статью с включенными зависимостями
        var createdArticle = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleType)
            .FirstOrDefaultAsync(a => a.Id == article.Id);

        if (createdArticle == null)
            throw new Exception("Failed to create article");

        return MapToDto(createdArticle);
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
            
            if (currentUser?.Role?.Name != "Admin")
                throw new ForbiddenException("You don't have permission to update this article");
        }

        if (!string.IsNullOrWhiteSpace(dto.Title))
        {
            article.Title = dto.Title;
            article.Slug = GenerateSlug(dto.Title);
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

        // Обновляем данные после сохранения
        await _context.Entry(article)
            .Reference(a => a.ArticleType)
            .LoadAsync();
        
        await _context.Entry(article)
            .Reference(a => a.Author)
            .LoadAsync();

        return MapToDto(article);
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
            
            if (currentUser?.Role?.Name != "Admin")
                throw new ForbiddenException("You don't have permission to delete this article");
        }

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();
    }

    public async Task<PaginatedResult<CommentDto>> GetArticleCommentsAsync(Guid articleId, int page = 1, int pageSize = 20)
    {
        var article = await _context.Articles.FindAsync(articleId);
        if (article == null)
            throw new NotFoundException($"Article with id {articleId} not found");

        var query = _context.Comments
            .Include(c => c.User)
            .Where(c => c.ContentId == articleId && c.ParentId == null)
            .OrderByDescending(c => c.CreatedOn);

        var totalCount = await query.CountAsync();

        var comments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var commentDtos = comments.Select(c => new CommentDto
        {
            Id = c.Id,
            Text = c.Text,
            UserId = c.UserId,
            Username = c.User.Username,
            AvatarId = c.User.AvatarId,
            ParentId = c.ParentId,
            ReplyCount = c.Replies.Count,
            CreatedOn = c.CreatedOn,
            ModifiedOn = c.ModifiedOn
        }).ToList();

        return new PaginatedResult<CommentDto>
        {
            Items = commentDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PaginatedResult<LikeDto>> GetArticleLikesAsync(Guid articleId, int page = 1, int pageSize = 20)
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
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var likeDtos = likes.Select(l => new LikeDto
        {
            Id = l.Id,
            UserId = l.UserId,
            Username = l.User.Username,
            AvatarId = l.User.AvatarId,
            CreatedOn = l.CreatedOn
        }).ToList();

        return new PaginatedResult<LikeDto>
        {
            Items = likeDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    private IQueryable<Article> BuildArticleQuery(ArticleFilterDto filter)
    {
        var query = _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleType)
            .Include(a => a.Likes)
            .Include(a => a.Comments)
            .Include(a => a.Views)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            if (Enum.TryParse<ContentStatus>(filter.Status, true, out var status))
            {
                query = query.Where(a => a.Status == status);
            }
        }
        else
        {
            query = query.Where(a => a.Status == ContentStatus.Published);
        }

        if (!string.IsNullOrWhiteSpace(filter.Type))
        {
            query = query.Where(a => a.ArticleType.Name == filter.Type);
        }

        if (filter.Author.HasValue)
        {
            query = query.Where(a => a.UserId == filter.Author.Value);
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
            "most_viewed" => query.OrderByDescending(a => a.Views.Count),
            _ => query.OrderByDescending(a => a.CreatedOn) 
        };

        return query;
    }

    private ArticleDto MapToDto(Article article)
    {
        // Используем null-условный оператор для безопасного доступа к коллекциям
        var likesCount = article.Likes?.Count ?? 0;
        var commentsCount = article.Comments?.Count ?? 0;
        var viewsCount = article.Views?.Count ?? 0;

        return new ArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            Slug = article.Slug,
            Excerpt = article.Excerpt,
            Body = article.Body,
            ReadingTimeMinutes = article.ReadingTimeMinutes,
            Status = article.Status.ToString(),
            ArticleType = article.ArticleType?.Name ?? string.Empty,
            ArticleTypeId = article.ArticleTypeId,
            AuthorId = article.UserId,
            AuthorName = article.Author?.Username ?? string.Empty,
            LikesCount = likesCount,
            CommentsCount = commentsCount,
            ViewsCount = viewsCount,
            CreatedOn = article.CreatedOn,
            ModifiedOn = article.ModifiedOn
        };
    }

    private string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        var slug = title.ToLowerInvariant()
            .Replace("ё", "е")
            .Replace(" ", "-")
            .Replace(",", "")
            .Replace(".", "")
            .Replace("!", "")
            .Replace("?", "")
            .Replace(":", "")
            .Replace(";", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("\"", "")
            .Replace("'", "")
            .Replace("&", "and")
            .Replace("@", "at")
            .Replace("#", "sharp")
            .Replace("%", "percent")
            .Replace("+", "plus")
            .Replace("=", "equals");

        // Удаляем множественные дефисы
        while (slug.Contains("--"))
        {
            slug = slug.Replace("--", "-");
        }

        // Удаляем дефисы в начале и конце
        slug = slug.Trim('-');

        // Если после обработки slug пустой, генерируем на основе даты
        if (string.IsNullOrEmpty(slug))
        {
            slug = $"article-{DateTime.UtcNow.Ticks}";
        }

        return slug;
    }

    // Дополнительные методы для работы со статусами
    public async Task<bool> PublishArticleAsync(Guid articleId, Guid currentUserId)
    {
        var article = await _context.Articles.FindAsync(articleId);
        if (article == null)
            throw new NotFoundException($"Article with id {articleId} not found");

        if (article.UserId != currentUserId)
        {
            var currentUser = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);
            
            if (currentUser?.Role?.Name != "Admin")
                throw new ForbiddenException("You don't have permission to publish this article");
        }

        article.Status = ContentStatus.Published;
        article.ModifiedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnpublishArticleAsync(Guid articleId, Guid currentUserId)
    {
        var article = await _context.Articles.FindAsync(articleId);
        if (article == null)
            throw new NotFoundException($"Article with id {articleId} not found");

        if (article.UserId != currentUserId)
        {
            var currentUser = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);
            
            if (currentUser?.Role?.Name != "Admin")
                throw new ForbiddenException("You don't have permission to unpublish this article");
        }

        article.Status = ContentStatus.Draft;
        article.ModifiedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ArchiveArticleAsync(Guid articleId, Guid currentUserId)
    {
        var article = await _context.Articles.FindAsync(articleId);
        if (article == null)
            throw new NotFoundException($"Article with id {articleId} not found");

        if (article.UserId != currentUserId)
        {
            var currentUser = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);
            
            if (currentUser?.Role?.Name != "Admin")
                throw new ForbiddenException("You don't have permission to archive this article");
        }

        article.Status = ContentStatus.Archived;
        article.ModifiedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}