namespace ChinaTown.Application.Dto.User;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid? AvatarId { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}
