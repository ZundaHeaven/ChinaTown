using ChinaTown.Domain.Enums;

namespace ChinaTown.Domain.Entities;

public abstract class Content : BaseEntity
{
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public string? Excerpt { get; set; }
    
    public ContentType ContentType { get; protected set; }
    
    public required Guid UserId { get; set; }
    public User Author { get; set; } = null!;
    
    public ContentStatus Status { get; set; } = ContentStatus.Draft;
    
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<View> Views { get; set; } = new List<View>();
}