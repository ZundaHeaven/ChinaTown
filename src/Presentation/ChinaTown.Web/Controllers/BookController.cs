using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChinaTown.Application.Dto.Book;
using ChinaTown.Application.Services;
using ChinaTown.Domain.Enums;
using ChinaTown.Domain.Exceptions;

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
        var userId = GetCurrentUserId();
        var book = await _bookService.CreateBookAsync(dto, userId);
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookUpdateDto dto)
    {
        var userId = GetCurrentUserId();
        var book = await _bookService.UpdateBookAsync(id, dto, userId);
        return Ok(book);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(Guid id)
    {
        var userId = GetCurrentUserId();
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
    public async Task<IActionResult> UploadBookFile(Guid id, IFormFile file)
    {
        var fileId = Guid.NewGuid();
        using var stream = file.OpenReadStream();
        await _bookService.UploadBookFileAsync(id, fileId, file.FileName, stream, (int)file.Length);
        return Ok(new { fileId });
    }

    [HttpGet("{id}/read")]
    public async Task<IActionResult> ReadBook(Guid id)
    {
        var fileBytes = await _bookService.ReadBookAsync(id);
        return File(fileBytes, "application/octet-stream");
    }

    [HttpGet("{id}/comments")]
    public async Task<IActionResult> GetComments(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _bookService.GetCommentsAsync(id, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}/likes")]
    public async Task<IActionResult> GetLikes(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _bookService.GetLikesAsync(id, page, pageSize);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyBooks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = GetCurrentUserId();
        var result = await _bookService.GetMyBooksAsync(userId, page, pageSize);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("archive")]
    public async Task<IActionResult> GetArchivedBooks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = GetCurrentUserId();
        var result = await _bookService.GetArchivedBooksAsync(userId, page, pageSize);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("{id}/publish")]
    public async Task<IActionResult> PublishBook(Guid id)
    {
        var userId = GetCurrentUserId();
        await _bookService.PublishBookAsync(id, userId);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/unpublish")]
    public async Task<IActionResult> UnpublishBook(Guid id)
    {
        var userId = GetCurrentUserId();
        await _bookService.UnpublishBookAsync(id, userId);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/archive")]
    public async Task<IActionResult> ArchiveBook(Guid id)
    {
        var userId = GetCurrentUserId();
        await _bookService.ArchiveBookAsync(id, userId);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> RestoreBook(Guid id)
    {
        var userId = GetCurrentUserId();
        await _bookService.RestoreBookAsync(id, userId);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("User not authenticated");
        return Guid.Parse(userId);
    }
}