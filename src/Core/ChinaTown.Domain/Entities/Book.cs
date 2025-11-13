namespace ChinaTown.Domain.Entities;

public class Book : Content
{
    public required string AuthorName { get; set; }
    public required string Description { get; set; }
    public required string FilePath { get; set; }
    public required int FileSizeBytes { get; set; }
    public required string CoverPath { get; set; }
    public required int PageAmount { get; set; }
    public required int YearOfPublish { get; set; }
    
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
}