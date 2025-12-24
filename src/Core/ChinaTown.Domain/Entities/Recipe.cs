using ChinaTown.Domain.Enums;

namespace ChinaTown.Domain.Entities;

public class Recipe : Content
{
    public Recipe()
    {
        ContentType = ContentType.Recipe;
    }
    
    public required RecipeDifficulty Difficulty { get; set; }
    public required string IngredientsJson { get; set; }
    public required string InstructionsJson { get; set; }
    public required int CookTimeMinutes { get; set; }
    public required string ImageUrl { get; set; }
    
    public virtual ICollection<RecipeTypeClaim> RecipeTypeClaims { get; set; } = new List<RecipeTypeClaim>();
    public virtual ICollection<RecipeRegion> RecipeRegions { get; set; } = new List<RecipeRegion>();
}