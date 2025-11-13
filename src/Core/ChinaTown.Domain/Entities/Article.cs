using ChinaTown.Domain.Enums;

namespace ChinaTown.Domain.Entities;

public class Article : Content
{
    public Article()
    {
        ContentType = ContentType.Article;
    }
    
    public required string Body { get; set; }
    public required int ReadingTimeMinutes { get; set; }
    
    public required Guid ArticleTypeId { get; set; }
    public ArticleType ArticleType { get; set; } = null!;
}