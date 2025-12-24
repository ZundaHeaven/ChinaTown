using System.ComponentModel.DataAnnotations;

namespace ChinaTown.Application.Dto.Article;

public class ArticleUpdateDto
{
    [MaxLength(500, ErrorMessage = "Title cannot exceed 500 characters")]
    public string? Title { get; set; }

    public string? Body { get; set; }

    [MaxLength(1000, ErrorMessage = "Excerpt cannot exceed 1000 characters")]
    public string? Excerpt { get; set; }

    public Guid? ArticleTypeId { get; set; }

    [Range(1, 480, ErrorMessage = "Reading time must be between 1 and 480 minutes")]
    public int? ReadingTimeMinutes { get; set; }

    public string? Status { get; set; }
}