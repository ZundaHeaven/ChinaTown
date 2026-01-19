using ChinaTown.Domain.Enums;

namespace ChinaTown.Application.Dto.Recipe;

public class RecipeFilterDto
{
    public string? Title { get; set; }
    public RecipeDifficulty? Difficulty { get; set; }
    public List<Guid>? RecipeTypeIds { get; set; }
    public List<Guid>? RegionIds { get; set; }
    public int? CookTimeMin { get; set; }
    public int? CookTimeMax { get; set; }
    public bool? Available { get; set; }
    public string? Sort { get; set; }
}