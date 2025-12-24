namespace ChinaTown.Domain.Entities;

public class Like : BaseEntity
{
    public Guid ContentId { get; set; }
    public Content Content { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}