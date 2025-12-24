using ChinaTown.Application.Data;
using ChinaTown.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace ChinaTown.Application.Services;

public class AvatarService : IAvatarService
{
    private readonly MongoDbContext _mongoContext;
    private readonly ApplicationDbContext _appContext;

    public AvatarService(MongoDbContext mongoContext, ApplicationDbContext appContext)
    {
        _mongoContext = mongoContext;
        _appContext = appContext;
    }

    public async Task<Guid> UploadAvatarAsync(Guid userId, IFormFile file)
    {
        var user = await _appContext.Users.FindAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        if (user.AvatarId.HasValue)
        {
            await DeleteAvatarAsync(user.AvatarId.Value);
        }

        var avatarId = Guid.NewGuid();
        using var stream = file.OpenReadStream();
        await _mongoContext.UploadFileAsync(avatarId, file.FileName, stream);

        user.AvatarId = avatarId;
        _appContext.Users.Update(user);
        await _appContext.SaveChangesAsync();

        return avatarId;
    }

    public async Task<byte[]> GetAvatarAsync(Guid avatarId)
    {
        return await _mongoContext.DownloadFileAsync(avatarId);
    }

    public async Task DeleteAvatarAsync(Guid avatarId)
    {
        await _mongoContext.DeleteFileAsync(avatarId);
    }
}