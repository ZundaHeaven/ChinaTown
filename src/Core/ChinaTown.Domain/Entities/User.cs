using ChinaTown.Domain.Enums;

namespace ChinaTown.Domain.Entities;

public class User : BaseEntity
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public Guid? AvatarId { get; set; }
    
    public required Role Role { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}