using ChinaTown.Application.Dto.Genre;
using ChinaTown.Domain.Enums;

namespace ChinaTown.Application.Dto.Book;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PageAmount { get; set; }
    public int YearOfPublish { get; set; }
    public int FileSizeBytes { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public List<GenreDto> Genres { get; set; } = new();
    public Guid CoverFileId { get; set; }
    public Guid BookFileId { get; set; }
    public string Status { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
}