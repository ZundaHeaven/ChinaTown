using ChinaTown.Application.Dto.User;

namespace ChinaTown.Application.Dto.Auth;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpires { get; set; }
    public DateTime RefreshTokenExpires { get; set; }
    public UserDto User { get; set; } = null!;
}