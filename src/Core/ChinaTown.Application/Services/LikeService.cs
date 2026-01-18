using AutoMapper;
using ChinaTown.Application.Data;
using ChinaTown.Application.Dto.Common;
using ChinaTown.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChinaTown.Application.Services;

public class LikeService : ILikeService
{
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _dbContext;

    public LikeService(IMapper mapper, ApplicationDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<LikeDto>> GetLikesAsync(Guid contentId)
    {
        var likes = await _dbContext.Likes
            .Where(a => a.ContentId == contentId)
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<LikeDto>>(likes);
    }

    public async Task ToggleLikeAsync(Guid contentId, Guid userId)
    {
        var like = await _dbContext.Likes
            .Where(a => a.ContentId == contentId && a.UserId == userId)
            .FirstOrDefaultAsync();

        if (like != null)
        {
            _dbContext.Likes.Remove(like);
        }
        else
        {
            var newLike = new Like
            {
                UserId = userId
            };
            
            _dbContext.Likes.Add(newLike);
        }
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<LikeDto>> GetUserLikesAsync(Guid userId)
    {
        var likes = await _dbContext.Likes
            .Where(a => a.UserId == userId)
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<LikeDto>>(likes);
    }
}