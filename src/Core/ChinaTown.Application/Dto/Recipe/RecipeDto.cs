using ChinaTown.Application.Dto.RecipeType;
using ChinaTown.Application.Dto.Region;
using ChinaTown.Domain.Enums;

namespace ChinaTown.Application.Dto.Recipe;

public class RecipeDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public RecipeDifficulty Difficulty { get; set; }
    public string Ingredients { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int CookTimeMinutes { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public Guid ImageId {get; set;}
    public List<RecipeTypeDto> RecipeTypes { get; set; } = new();
    public List<RegionDto> Regions { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
}
