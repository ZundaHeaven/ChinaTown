using System.Security.Claims;
using ChinaTown.Domain.Entities;

namespace ChinaTown.Application.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    DateTime GetAccessTokenExpiration();
    DateTime GetRefreshTokenExpiration();
}