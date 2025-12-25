namespace ChinaTown.Application.Dto.Book;

public class BookFilterDto
{
    public string? Title { get; set; }
    public string? AuthorName { get; set; }
    public List<Guid>? GenreIds { get; set; }
    public int? YearMin { get; set; }
    public int? YearMax { get; set; }
    public int? PagesMin { get; set; }
    public int? PagesMax { get; set; }
    public string? Language { get; set; }
    public bool? Available { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}