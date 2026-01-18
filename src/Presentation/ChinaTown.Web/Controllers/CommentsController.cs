using ChinaTown.Application.Dto.Book;
using ChinaTown.Application.Dto.Common;
using ChinaTown.Application.Services;
using ChinaTown.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChinaTown.Web.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("content/{id}")]
    public async Task<IActionResult> GetCommentsByContent(Guid id)
    {
        var result = await _commentService.GetCommentsAsync(id);
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddComment([FromBody] CommentAddDto dto)
    {
        var userId = ControllerHelper.GetUserIdFromPrincipals(User);
        var comment = await _commentService.CreateCommentAsync(userId, dto);
        return CreatedAtAction(null, new { id = comment.Id }, comment);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(Guid id, [FromBody] CommentUpdateDto dto)
    {
        var userId = ControllerHelper.GetUserIdFromPrincipals(User);
        var userRole = ControllerHelper.GetRoleFromClaims(User);
        var comment = await _commentService.UpdateCommentAsync(id, dto, userId, userRole);
        return Ok(comment);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(Guid id)
    {
        var userId = ControllerHelper.GetUserIdFromPrincipals(User);
        var userRole = ControllerHelper.GetRoleFromClaims(User);
        await _commentService.DeleteCommentAsync(id, userId, userRole);
        return NoContent();
    }
    
    [Authorize]
    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetUserComments(Guid id)
    {
        var comments = await _commentService.GetUserCommentsAsync(id);
        return Ok(comments);
    }
}