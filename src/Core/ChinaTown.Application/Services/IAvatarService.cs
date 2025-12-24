using Microsoft.AspNetCore.Http;

namespace ChinaTown.Application.Services;

public interface IAvatarService
{
    Task<Guid> UploadAvatarAsync(Guid userId, IFormFile file);
    Task<byte[]> GetAvatarAsync(Guid avatarId);
    Task DeleteAvatarAsync(Guid avatarId);
}