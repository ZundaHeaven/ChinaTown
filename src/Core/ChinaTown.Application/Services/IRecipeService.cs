using ChinaTown.Application.Dto.Common;
using ChinaTown.Application.Dto.Recipe;
using ChinaTown.Domain.Enums;

namespace ChinaTown.Application.Services;

public interface IRecipeService
{
    Task<IEnumerable<RecipeDto>> GetRecipesAsync(RecipeFilterDto filter);
    Task<RecipeDto> GetRecipeAsync(Guid id);
    Task<RecipeDto> CreateRecipeAsync(RecipeCreateDto dto, Guid userId);
    Task<RecipeDto> UpdateRecipeAsync(Guid id, RecipeUpdateDto dto, Guid userId);
    Task DeleteRecipeAsync(Guid id, Guid userId);
    
    Task<IEnumerable<RecipeDto>> GetMyRecipesAsync(Guid userId);
    Task<IEnumerable<RecipeDto>> GetArchivedRecipesAsync(Guid userId);

    
    Task<IEnumerable<CommentDto>> GetCommentsAsync(Guid recipeId);
    Task<IEnumerable<LikeDto>> GetLikesAsync(Guid recipeId);
    
    Task UploadImageAsync(Guid recipeId, Guid fileId, string fileName, Stream stream);
    Task<byte[]> GetImageAsync(Guid recipeId);
    Task<bool> ChangeStatusAsync(Guid bookId, Guid currentUserId, ContentStatus status);
    
}