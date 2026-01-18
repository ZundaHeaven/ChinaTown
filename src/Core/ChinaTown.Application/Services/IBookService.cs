using ChinaTown.Application.Dto.Book;
using ChinaTown.Application.Dto.Common;

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
    Task PublishBookAsync(Guid bookId, Guid userId);
    Task UnpublishBookAsync(Guid bookId, Guid userId);
    Task ArchiveBookAsync(Guid bookId, Guid userId);
    Task RestoreBookAsync(Guid bookId, Guid userId);
}