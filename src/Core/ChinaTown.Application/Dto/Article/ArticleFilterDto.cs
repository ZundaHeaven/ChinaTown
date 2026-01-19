namespace ChinaTown.Application.Dto.Article;

public class ArticleFilterDto
{
    public string? Search { get; set; }
    public string? Type { get; set; }
    public Guid? AuthorId { get; set; }
    public string? Sort { get; set; } = "newest";
    public string? Status { get; set; }
}