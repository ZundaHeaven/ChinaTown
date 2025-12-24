using System.ComponentModel.DataAnnotations;

namespace ChinaTown.Application.Dto.Auth;

public class LoginRequestDto
{
    [Required]
    public string UsernameOrEmail { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}