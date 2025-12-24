using ChinaTown.Application.Data;
using ChinaTown.Application.Dto.User;
using ChinaTown.Domain.Entities;
using ChinaTown.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChinaTown.Application.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .Include(u => u.Role)
            .ToListAsync();

        return users.Select(u => MapToUserDto(u, u.Role));
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new NotFoundException("User not found");

        return MapToUserDto(user, user.Role);
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UserUpdateDto dto, Guid currentUserId)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new NotFoundException("User not found");

        if (user.Id != currentUserId && !await IsAdminAsync(currentUserId))
            throw new ForbiddenException("You don't have permission to update this user");

        if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != user.Username)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username && u.Id != id))
                throw new BadRequestException("Username already exists");
            user.Username = dto.Username;
        }

        if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id))
                throw new BadRequestException("Email already exists");
            user.Email = dto.Email;
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return MapToUserDto(user, user.Role);
    }

    public async Task DeleteUserAsync(Guid id, Guid currentUserId)
    {
        if (!await IsAdminAsync(currentUserId))
            throw new ForbiddenException("Only administrators can delete users");

        if (id == currentUserId)
            throw new BadRequestException("You cannot delete your own account");

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new NotFoundException("User not found");

        if (user.Role.Name == "Admin")
        {
            var adminCount = await _context.Users
                .Include(u => u.Role)
                .CountAsync(u => u.Role.Name == "Admin");

            if (adminCount <= 1)
                throw new BadRequestException("Cannot delete the last administrator");
        }

        var refreshTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == id)
            .ToListAsync();
        
        _context.RefreshTokens.RemoveRange(refreshTokens);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    private async Task<bool> IsAdminAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);
        return user?.Role.Name == "Admin";
    }

    private UserDto MapToUserDto(User user, Role role)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            AvatarId = user.AvatarId,
            Role = role.Name,
            CreatedOn = user.CreatedOn,
            ModifiedOn = user.ModifiedOn
        };
    }
}