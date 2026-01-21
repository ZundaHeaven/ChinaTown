using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChinaTown.Application.Dto.Auth;
using ChinaTown.Application.Dto.User;
using ChinaTown.Application.Services;
using ChinaTown.Web.Extensions;

namespace ChinaTown.Web.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> Refresh(RefreshTokenRequestDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(LogoutRequestDto dto)
    {
        var userId = ControllerHelper.GetUserIdFromPrincipals(User);        
        await _authService.LogoutAsync(userId, dto.RefreshToken);
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = ControllerHelper.GetUserIdFromPrincipals(User);
        var user = await _authService.GetCurrentUserAsync(userId);
        return Ok(user);
    }
}