namespace ChinaTown.Domain.Entities;

public class RecipeRegion
{
    public required Guid RegionId { get; set; }
    public Region Region { get; set; } = null!;
    
    public required Guid RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
}