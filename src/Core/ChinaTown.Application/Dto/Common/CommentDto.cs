namespace ChinaTown.Application.Dto.Common;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public Guid? AvatarId { get; set; }
    public Guid? ParentId { get; set; }
    public int ReplyCount { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}