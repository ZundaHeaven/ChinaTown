using AutoMapper;
using ChinaTown.Application.Data;
using ChinaTown.Application.Dto.Auth;
using ChinaTown.Application.Dto.User;
using ChinaTown.Domain.Entities;
using ChinaTown.Domain.Enums;
using ChinaTown.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ChinaTown.Application.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(
        ApplicationDbContext context,
        IJwtTokenService jwtTokenService,
        IMapper mapper,
        IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new BadRequestException("Email already exists");

        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            throw new BadRequestException("Username already exists");
        
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Role = Role.User,
            PasswordHash = _passwordHasher.HashPassword(null!, dto.Password)
        };

        var entity = await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        user = entity.Entity;

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenExpires = _jwtTokenService.GetRefreshTokenExpiration();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            Expires = refreshTokenExpires,
            IsRevoked = false,
            IsUsed = false,
            UserId = entity.Entity.Id
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpires = _jwtTokenService.GetAccessTokenExpiration(),
            RefreshTokenExpires = refreshTokenExpires,
            User = _mapper.Map<UserDto>(user)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => 
                u.Username == dto.UsernameOrEmail || 
                u.Email == dto.UsernameOrEmail);

        if (user == null)
            throw new UnauthorizedException("Invalid credentials");

        var result = _passwordHasher.VerifyHashedPassword(
            user, user.PasswordHash, dto.Password);

        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedException("Invalid credentials");

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenExpires = _jwtTokenService.GetRefreshTokenExpiration();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            Expires = refreshTokenExpires,
            IsRevoked = false,
            IsUsed = false,
            UserId = user.Id
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpires = _jwtTokenService.GetAccessTokenExpiration(),
            RefreshTokenExpires = refreshTokenExpires,
            User = _mapper.Map<UserDto>(user)
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var refreshTokenEntity = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (refreshTokenEntity == null || 
            refreshTokenEntity.IsRevoked || 
            refreshTokenEntity.IsUsed || 
            refreshTokenEntity.Expires < DateTime.UtcNow)
            throw new UnauthorizedException("Invalid refresh token");

        refreshTokenEntity.IsUsed = true;
        _context.RefreshTokens.Update(refreshTokenEntity);

        var user = refreshTokenEntity.User;
        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
        var newRefreshTokenExpires = _jwtTokenService.GetRefreshTokenExpiration();

        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            Expires = newRefreshTokenExpires,
            IsRevoked = false,
            IsUsed = false,
            UserId = user.Id
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            AccessTokenExpires = _jwtTokenService.GetAccessTokenExpiration(),
            RefreshTokenExpires = newRefreshTokenExpires,
            User = _mapper.Map<UserDto>(user)
        };
    }

    public async Task LogoutAsync(Guid userId, string refreshToken)
    {
        var refreshTokenEntity = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => 
                rt.UserId == userId && 
                rt.Token == refreshToken && 
                !rt.IsRevoked);

        if (refreshTokenEntity != null)
        {
            refreshTokenEntity.IsRevoked = true;
            _context.RefreshTokens.Update(refreshTokenEntity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<UserDto> GetCurrentUserAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new NotFoundException("User not found");

        return _mapper.Map<UserDto>(user);
    }
}