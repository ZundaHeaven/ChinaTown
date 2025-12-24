namespace ChinaTown.Application.Dto.Common;

public class LikeDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public Guid? AvatarId { get; set; }
    public DateTime CreatedOn { get; set; }
}