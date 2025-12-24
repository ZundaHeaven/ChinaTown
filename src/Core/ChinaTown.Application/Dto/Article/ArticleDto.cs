namespace ChinaTown.Application.Dto.Article;

public class ArticleDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string Body { get; set; } = string.Empty;
    public int ReadingTimeMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ArticleType { get; set; } = string.Empty;
    public Guid ArticleTypeId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public int ViewsCount { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}