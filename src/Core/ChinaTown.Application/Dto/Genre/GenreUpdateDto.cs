using System.ComponentModel.DataAnnotations;

namespace ChinaTown.Application.Dto.Genre;

public class GenreUpdateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}