namespace ChinaTown.Domain.Entities;

public class RecipeRegion
{
    public Guid RegionId { get; set; }
    public Region Region { get; set; } = null!;
    
    public Guid RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
}