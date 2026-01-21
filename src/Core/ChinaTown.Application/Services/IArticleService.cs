using ChinaTown.Application.Dto.Article;
using ChinaTown.Application.Dto.Common;
using ChinaTown.Domain.Enums;

namespace ChinaTown.Application.Services;

public interface IArticleService
{
    Task<IEnumerable<ArticleDto>> GetArticlesAsync(ArticleFilterDto filter);
    Task<ArticleDto?> GetArticleByIdAsync(Guid id);
    Task<ArticleDto> CreateArticleAsync(ArticleCreateDto dto, Guid authorId);
    Task<ArticleDto> UpdateArticleAsync(Guid id, ArticleUpdateDto dto, Guid currentUserId);
    Task DeleteArticleAsync(Guid id, Guid currentUserId);
    Task<IEnumerable<CommentDto>> GetArticleCommentsAsync(Guid articleId);
    Task<IEnumerable<LikeDto>> GetArticleLikesAsync(Guid articleId);
    Task<bool> ChangeStatusAsync(Guid bookId, Guid currentUserId, ContentStatus status);
}
