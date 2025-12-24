namespace ChinaTown.Domain.Entities;

public class View : BaseEntity
{
    public Guid ContentId { get; set; }
    public Content Content { get; set; } = null!;
        
    public Guid? UserId { get; set; }
    public User? User { get; set; }
        
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}