using ChinaTown.Application.Dto.Auth;
using ChinaTown.Application.Dto.User;

namespace ChinaTown.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(Guid userId, string refreshToken);
    Task<UserDto> GetCurrentUserAsync(Guid userId);
}