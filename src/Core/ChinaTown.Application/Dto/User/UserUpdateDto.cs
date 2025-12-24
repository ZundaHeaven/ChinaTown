using System.ComponentModel.DataAnnotations;

namespace ChinaTown.Application.Dto.User;

public class UserUpdateDto
{
    [MaxLength(100)]
    public string? Username { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
}