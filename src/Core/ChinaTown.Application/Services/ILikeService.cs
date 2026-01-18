using ChinaTown.Application.Dto.Common;

namespace ChinaTown.Application.Services;

public interface ILikeService
{
    Task<IEnumerable<LikeDto>> GetLikesAsync(Guid contentId);
    Task ToggleLikeAsync(Guid contentId, Guid userId);
    Task<IEnumerable<LikeDto>> GetUserLikesAsync(Guid userId);
}