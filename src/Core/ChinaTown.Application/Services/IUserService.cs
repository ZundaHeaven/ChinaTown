using ChinaTown.Application.Dto.User;

namespace ChinaTown.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(Guid id);
    Task<UserDto> UpdateUserAsync(Guid id, UserUpdateDto dto, Guid currentUserId);
    Task DeleteUserAsync(Guid id, Guid currentUserId);
}