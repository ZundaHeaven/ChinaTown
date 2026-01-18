namespace ChinaTown.Domain.Entities;

public class Comment : BaseEntity
{
    public required string Text { get; set; }
    
    public Guid ContentId { get; set; }
    public Content Content { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}