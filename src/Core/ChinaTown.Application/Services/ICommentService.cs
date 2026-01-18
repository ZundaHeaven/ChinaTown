using ChinaTown.Application.Dto.Common;

namespace ChinaTown.Application.Services;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetCommentsAsync(Guid contentId);
    Task<CommentDto> CreateCommentAsync(Guid userId, CommentAddDto comment);
    Task<CommentDto> UpdateCommentAsync(Guid commentId, CommentUpdateDto comment, Guid userId, string userRole);
    Task DeleteCommentAsync(Guid commentId, Guid userId, string userRole);
    Task<IEnumerable<CommentDto>> GetUserCommentsAsync(Guid userId);
}