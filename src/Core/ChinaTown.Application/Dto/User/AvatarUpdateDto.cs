using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ChinaTown.Application.Dto.User;

public class AvatarUpdateDto
{
    [Required]
    public IFormFile? AvatarFile { get; set; }
}