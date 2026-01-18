namespace ChinaTown.Application.Models.Article;

public class ArticleQuery
{
    public string? Search { get; set; }
    public string? Type { get; set; }
    public Guid? AuthorId { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public string? Status { get; set; }
}