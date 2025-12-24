namespace ChinaTown.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public required string Token { get; set; }
    public required DateTime Expires { get; set; }
    public bool IsRevoked { get; set; }
    public bool IsUsed { get; set; }
    
    public required Guid UserId { get; set; }
    public User User { get; set; } = null!;
}