using System.ComponentModel.DataAnnotations;

namespace ChinaTown.Application.Dto.Article;


public class ArticleTypeCreateDto
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;
}