namespace ChinaTown.Domain.Entities;

public class Region : BaseEntity
{
    public required string Name { get; set; }
    public virtual ICollection<RecipeRegion> RecipeRegions { get; set; } = new List<RecipeRegion>();
}