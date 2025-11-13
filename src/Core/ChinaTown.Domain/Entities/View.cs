namespace ChinaTown.Domain.Entities;

public class View : BaseEntity
{
    public required Guid ContentId { get; set; }
    public Content Content { get; set; } = null!;
        
    public int? UserId { get; set; }
    public User User { get; set; } = null!;
        
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}