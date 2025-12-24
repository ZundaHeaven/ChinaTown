using ChinaTown.Application.Dto.Article;
using ChinaTown.Application.Dto.Common;

namespace ChinaTown.Application.Services;

public interface IArticleService
{
    Task<PaginatedResult<ArticleDto>> GetArticlesAsync(ArticleFilterDto filter);
    Task<ArticleDto?> GetArticleByIdAsync(Guid id);
    Task<ArticleDto> CreateArticleAsync(ArticleCreateDto dto, Guid authorId);
    Task<ArticleDto> UpdateArticleAsync(Guid id, ArticleUpdateDto dto, Guid currentUserId);
    Task DeleteArticleAsync(Guid id, Guid currentUserId);
    Task<PaginatedResult<CommentDto>> GetArticleCommentsAsync(Guid articleId, int page = 1, int pageSize = 20);
    Task<PaginatedResult<LikeDto>> GetArticleLikesAsync(Guid articleId, int page = 1, int pageSize = 20);
}
