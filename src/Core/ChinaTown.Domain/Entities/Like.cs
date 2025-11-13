using System.Net.Mime;

namespace ChinaTown.Domain.Entities;

public class Like : BaseEntity
{
    public required Guid ContentId { get; set; }
    public Content Content { get; set; } = null!;
    
    public required Guid UserId { get; set; }
    public User User { get; set; } = null!;
}