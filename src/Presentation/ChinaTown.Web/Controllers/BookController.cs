using ChinaTown.Application.Dto.Article;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChinaTown.Application.Dto.Book;
using ChinaTown.Application.Services;
using ChinaTown.Domain.Enums;
using ChinaTown.Domain.Exceptions;
using ChinaTown.Web.Extensions;

namespace ChinaTown.Web.Controllers;

[ApiController]
[Route("api/books")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBooks([FromQuery] BookFilterDto filter)
    {
        var result = await _bookService.GetBooksAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(Guid id)
    {
        var book = await _bookService.GetBookAsync(id);
        return Ok(book);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] BookCreateDto dto)
    {
        var userId = ControllerHelper.GetUserIdFromPrincipals(User);
        var book = await _bookService.CreateBookAsync(dto, userId);
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookUpdateDto dto)
    {
        var userId = ControllerHelper.GetUserIdFromPrincipals(User);
        var book = await _bookService.UpdateBookAsync(id, dto, userId);
        return Ok(book);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(Guid id)
    {
        var userId = ControllerHelper.GetUserIdFromPrincipals(User);
        await _bookService.DeleteBookAsync(id, userId);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/cover")]
    public async Task<IActionResult> UploadCover(Guid id, IFormFile file)
    {
        var fileId = Guid.NewGuid();
        using var stream = file.OpenReadStream();
        await _bookService.UploadCoverAsync(id, fileId, file.FileName, stream);
        return Ok(new { fileId });
    }

    [Authorize]
    [HttpPost("{id}/file")]
    public async Task<IActionResult> UploadBookFile(Guid id, IFormFile bookFile)
    {
        var fileId = Guid.NewGuid();
        var stream = bookFile.OpenReadStream();
        await _bookService.UploadBookFileAsync(id, fileId, bookFile.FileName, stream, (int)bookFile.Length);
        stream.Close();
        return Ok(new { fileId });
    }

    [HttpGet("{id}/read")]
    public async Task<IActionResult> ReadBook(Guid id)
    {
        var fileBytes = await _bookService.ReadBookAsync(id);
        return File(fileBytes, "application/octet-stream");
    }

    [HttpGet("{id}/comments")]
    public async Task<IActionResult> GetComments(Guid id)
    {
        var result = await _bookService.GetCommentsAsync(id);
        return Ok(result);
    }

    [HttpGet("{id}/likes")]
    public async Task<IActionResult> GetLikes(Guid id)
    {
        var result = await _bookService.GetLikesAsync(id);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyBooks()
    {
        var userId = ControllerHelper.GetUserIdFromPrincipals(User);
        var result = await _bookService.GetMyBooksAsync(userId);
        return Ok(result);
    }
    
    [HttpPatch("{id:guid}/status")]
    [Authorize]
    public async Task<ActionResult> ChangeStatus(Guid id, [FromBody] string status)
    {
        var currentUserId = ControllerHelper.GetUserIdFromPrincipals(User);

        if (status != nameof(ContentStatus.Archived) &&
            status != nameof(ContentStatus.Draft) &&
            status != nameof(ContentStatus.Published))
        {
            return BadRequest(new {message = "Invalid status"});
        }
        
        ContentStatus contentStatus = status == nameof(ContentStatus.Archived) ? ContentStatus.Archived 
            : status == nameof(ContentStatus.Draft) ? ContentStatus.Draft 
            : ContentStatus.Archived;
        
        await _bookService.ChangeStatusAsync(id, currentUserId, contentStatus);
        
        return Ok(new { message = "Book status changed successfully" });
    }
}