using ChinaTown.Application.Dto.Book;
using ChinaTown.Application.Dto.Common;
using ChinaTown.Domain.Enums;

namespace ChinaTown.Application.Services;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetBooksAsync(BookFilterDto filter);
    Task<BookDto> GetBookAsync(Guid id);
    Task<BookDto> CreateBookAsync(BookCreateDto dto, Guid userId);
    Task<BookDto> UpdateBookAsync(Guid id, BookUpdateDto dto, Guid userId);
    Task DeleteBookAsync(Guid id, Guid userId);
    Task UploadCoverAsync(Guid bookId, Guid fileId, string fileName, Stream stream);
    Task UploadBookFileAsync(Guid bookId, Guid fileId, string fileName, Stream stream, int fileSize);
    Task<byte[]> ReadBookAsync(Guid bookId);
    Task<IEnumerable<CommentDto>> GetCommentsAsync(Guid bookId);
    Task<IEnumerable<LikeDto>> GetLikesAsync(Guid bookId);
    Task<IEnumerable<BookDto>> GetMyBooksAsync(Guid userId);
    Task<IEnumerable<BookDto>> GetArchivedBooksAsync(Guid userId);
    Task<bool> ChangeStatusAsync(Guid bookId, Guid currentUserId, ContentStatus status);
}