using System.ComponentModel.DataAnnotations;

namespace ChinaTown.Application.Dto.Book;

public class BookCreateDto
{
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string AuthorName { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public int PageAmount { get; set; }
    [Required]
    public int YearOfPublish { get; set; }
    public List<Guid> GenreIds { get; set; } = new();
}