using ChinaTown.Domain.Enums;

namespace ChinaTown.Application.Dto.Common;

public class LikeDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public Guid? AvatarId { get; set; }
    public string ContentType { get; set; }
    public Guid ContentId { get; set; }
    public DateTime CreatedOn { get; set; }
    public string Title = string.Empty;
    public string Slug { get; set; }
    public string? Excerpt { get; set; }
    public required Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = null!;
    public string Status { get; set; }
}