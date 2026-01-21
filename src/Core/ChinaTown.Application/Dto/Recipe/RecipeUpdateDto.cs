using System.ComponentModel.DataAnnotations;
using ChinaTown.Domain.Enums;

namespace ChinaTown.Application.Dto.Recipe;

public class RecipeUpdateDto
{
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [EnumDataType(typeof(RecipeDifficulty))]
    public RecipeDifficulty Difficulty { get; set; }
    [Required]
    public string Status { get; set; } = string.Empty;
    
    [Required]
    public string Ingredients { get; set; } = string.Empty;
    
    [Required]
    public string Instructions { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 1000)]
    public int CookTimeMinutes { get; set; }
    
    public List<Guid> RecipeTypeIds { get; set; } = new();
    public List<Guid> RegionIds { get; set; } = new();
}