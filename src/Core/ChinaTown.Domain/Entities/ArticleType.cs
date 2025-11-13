namespace ChinaTown.Domain.Entities;

public class ArticleType : BaseEntity
{
    public required string Name { get; set; } = null!;
    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}