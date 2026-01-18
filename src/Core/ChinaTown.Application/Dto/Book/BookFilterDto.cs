namespace ChinaTown.Application.Dto.Book;

public class BookFilterDto
{
    public string? Title { get; set; }
    public string? AuthorName { get; set; }
    public List<Guid>? GenreIds { get; set; }
    public int? YearMin { get; set; }
    public int? YearMax { get; set; }
    public bool? Available { get; set; }
    public string? Sort { get; set; }
}