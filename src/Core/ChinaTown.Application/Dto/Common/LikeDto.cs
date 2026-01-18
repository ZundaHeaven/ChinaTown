using ChinaTown.Domain.Enums;

namespace ChinaTown.Application.Dto.Common;

public class LikeDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public Guid? AvatarId { get; set; }
    public ContentType ContentType { get; set; }
    public Guid ContentId { get; set; }
    public DateTime CreatedOn { get; set; }
}