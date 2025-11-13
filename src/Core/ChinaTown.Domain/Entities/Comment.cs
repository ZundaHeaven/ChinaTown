namespace ChinaTown.Domain.Entities;

public class Comment : BaseEntity
{
    public Guid ContentId { get; set; }
    public Content Content { get; set; } = null!;
        
    public Guid? ParentId { get; set; }
    public Comment? Parent { get; set; }
    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}