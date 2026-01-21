using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChinaTown.Application.Dto.Common;
using ChinaTown.Application.Services;
using ChinaTown.Domain.Exceptions;
using ChinaTown.Web.Extensions;

namespace ChinaTown.Web.Controllers;

[ApiController]
[Route("api/likes")]
public class LikeController : ControllerBase
{
    private readonly ILikeService _likeService;

    public LikeController(ILikeService likeService)
    {
        _likeService = likeService;
    }

    [HttpGet("content/{contentId}")]
    public async Task<IActionResult> GetLikesForContent(Guid contentId)
    {
        var result = await _likeService.GetLikesAsync(contentId);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyLikes()
    {
        var userId = ControllerHelper.GetUserIdFromPrincipals(User);
        var result = await _likeService.GetUserLikesAsync(userId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("content/{contentId}/toggle")]
    public async Task<IActionResult> ToggleLike(Guid contentId)
    {
        var userId = ControllerHelper.GetUserIdFromPrincipals(User);
        await _likeService.ToggleLikeAsync(contentId, userId);
        
        var updatedLikes = await _likeService.GetLikesAsync(contentId);
        return Ok(new 
        { 
            likes = updatedLikes,
            likesCount = updatedLikes.Count()
        });
    }

    [Authorize]
    [HttpGet("content/{contentId}/check")]
    public async Task<IActionResult> CheckIfLiked(Guid contentId)
    {
        var userId = ControllerHelper.GetUserIdFromPrincipals(User);
        var likes = await _likeService.GetLikesAsync(contentId);
        var isLiked = likes.Any(like => like.UserId == userId);
        
        return Ok(new { isLiked });
    }
}