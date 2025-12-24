using ChinaTown.Application.Dto.Article;

namespace ChinaTown.Application.Services;

public interface IArticleTypeService
{
    Task<IEnumerable<ArticleTypeDto>> GetAllArticleTypesAsync();
    Task<ArticleTypeDto?> GetArticleTypeByIdAsync(Guid id);
    Task<ArticleTypeDto> CreateArticleTypeAsync(ArticleTypeCreateDto dto);
    Task<ArticleTypeDto> UpdateArticleTypeAsync(Guid id, ArticleTypeUpdateDto dto);
    Task DeleteArticleTypeAsync(Guid id);
}
