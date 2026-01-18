using AutoMapper;
using ChinaTown.Application.Data;
using ChinaTown.Application.Dto.Common;
using ChinaTown.Domain.Entities;
using ChinaTown.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChinaTown.Application.Services;

public class CommentService : ICommentService
{
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _dbContext;

    public CommentService(IMapper mapper, ApplicationDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<CommentDto>> GetCommentsAsync(Guid contentId)
    {
        var comments = await _dbContext.Comments
            .Include(c => c.Content)
            .Include(c => c.User)
            .Where(c => c.ContentId == contentId)
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }
    
    public async Task<CommentDto> CreateCommentAsync(Guid userId, CommentAddDto comment)
    {
        var newComment = new Comment
        {
            ContentId = comment.ContentId,
            UserId = userId,
            Text = comment.Content,
        };
        
        _dbContext.Comments.Add(newComment);
        await _dbContext.SaveChangesAsync();
        
        await _dbContext.Entry(newComment)
            .Reference(c => c.Content)
            .LoadAsync();
        
        await _dbContext.Entry(newComment)
            .Reference(c => c.User)
            .LoadAsync();
        
        return _mapper.Map<CommentDto>(newComment);
    }

    public async Task<CommentDto> UpdateCommentAsync(Guid commentId, CommentUpdateDto comment, Guid userId, string userRole)
    {
        var foundComment = await _dbContext.Comments
            .Include(c => c.Content)
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == commentId);
        
        if(foundComment == null)
            throw new NotFoundException("Comment not found");
        
        if(userId != foundComment.UserId || userRole != "Admin")
            throw new ForbiddenException("Not enough permission to edit comment");
        
        foundComment.Text = comment.Content;
        _dbContext.Comments.Update(foundComment);
        await _dbContext.SaveChangesAsync();
        
        return _mapper.Map<CommentDto>(foundComment);
    }

    public async Task DeleteCommentAsync(Guid commentId, Guid userId, string userRole)
    {
        var foundComment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

        if (foundComment == null)
            throw new NotFoundException("Comment not found");

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new NotFoundException("User not found");

        if (foundComment.UserId != userId || userRole != "Admin")
            throw new ForbiddenException("Not enough permission to delete comment");

        _dbContext.Comments.Remove(foundComment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<CommentDto>> GetUserCommentsAsync(Guid userId)
    {
        var comments = await _dbContext.Comments
            .Include(c => c.Content)
            .Include(c => c.User)
            .Where(c => c.UserId == userId)
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }
}