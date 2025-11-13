namespace ChinaTown.Domain.Entities;

public class User : BaseEntity
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? AvatarPath { get; set; }
    
    public required Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<View> Views { get; set; } = new List<View>();
    public ICollection<Content> Contents { get; set; } = new List<Content>();
}