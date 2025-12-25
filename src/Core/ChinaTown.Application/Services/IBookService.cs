using ChinaTown.Application.Dto.Book;
using ChinaTown.Application.Dto.Common;

namespace ChinaTown.Application.Services;

public interface IBookService
{
    Task<PaginatedResult<BookDto>> GetBooksAsync(BookFilterDto filter);
    Task<BookDto> GetBookAsync(Guid id);
    Task<BookDto> CreateBookAsync(BookCreateDto dto, Guid userId);
    Task<BookDto> UpdateBookAsync(Guid id, BookUpdateDto dto, Guid userId);
    Task DeleteBookAsync(Guid id, Guid userId);
    Task UploadCoverAsync(Guid bookId, Guid fileId, string fileName, Stream stream);
    Task UploadBookFileAsync(Guid bookId, Guid fileId, string fileName, Stream stream, int fileSize);
    Task<byte[]> ReadBookAsync(Guid bookId);
    Task<PaginatedResult<CommentDto>> GetCommentsAsync(Guid bookId, int page, int pageSize);
    Task<PaginatedResult<LikeDto>> GetLikesAsync(Guid bookId, int page, int pageSize);
    Task<PaginatedResult<BookDto>> GetMyBooksAsync(Guid userId, int page, int pageSize);
    Task<PaginatedResult<BookDto>> GetArchivedBooksAsync(Guid userId, int page, int pageSize);
    Task PublishBookAsync(Guid bookId, Guid userId);
    Task UnpublishBookAsync(Guid bookId, Guid userId);
    Task ArchiveBookAsync(Guid bookId, Guid userId);
    Task RestoreBookAsync(Guid bookId, Guid userId);
}