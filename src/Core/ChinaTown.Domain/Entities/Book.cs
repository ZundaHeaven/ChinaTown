using ChinaTown.Domain.Enums;

namespace ChinaTown.Domain.Entities;

public class Book : Content
{
    public Book()
    {
        ContentType = ContentType.Book;
    }
    
    public required string AuthorName { get; set; }
    public required string Description { get; set; }
    public required Guid BookFileId { get; set; }
    public required int FileSizeBytes { get; set; }
    public required string CoverPath { get; set; }
    public required int PageAmount { get; set; }
    public required int YearOfPublish { get; set; }
    
    public virtual ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
}