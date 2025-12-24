using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChinaTown.Application.Dto.User;
using ChinaTown.Application.Services;
using ChinaTown.Domain.Exceptions;
using ChinaTown.Web.Extensions;

namespace ChinaTown.Web.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAvatarService _avatarService;

    public UsersController(IUserService userService, IAvatarService avatarService)
    {
        _userService = userService;
        _avatarService = avatarService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, UserUpdateDto dto)
    {
        var currentUserId = ControllerHelper.GetUserIdFromPrincipals(User);
        var user = await _userService.UpdateUserAsync(id, dto, currentUserId);
        return Ok(user);
    }

    [HttpPatch("{id:guid}/avatar")]
    public async Task<ActionResult<Guid>> UpdateAvatar(Guid id, [FromForm] AvatarUpdateDto dto)
    {
        if (dto.AvatarFile == null || dto.AvatarFile.Length == 0)
            throw new BadRequestException("Avatar file is required");

        var currentUserId = ControllerHelper.GetUserIdFromPrincipals(User);
        
        if (id != currentUserId && !User.IsInRole("Admin"))
            throw new ForbiddenException("You can only update your own avatar");

        var avatarId = await _avatarService.UploadAvatarAsync(id, dto.AvatarFile);
        return Ok(avatarId);
    }

    [HttpGet("{id:guid}/avatar")]
    public async Task<IActionResult> GetAvatar(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        
        if (!user.AvatarId.HasValue)
            return NotFound();

        var avatarBytes = await _avatarService.GetAvatarAsync(user.AvatarId.Value);
        return File(avatarBytes, "image/jpeg");
    }
    
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var currentUserId = ControllerHelper.GetUserIdFromPrincipals(User);
        
        await _userService.DeleteUserAsync(id, currentUserId);
        return NoContent();
    }
}