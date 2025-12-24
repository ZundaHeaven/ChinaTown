namespace ChinaTown.Domain.Entities;

public class RecipeType : BaseEntity
{
    public required string Name { get; set; }
    public virtual ICollection<RecipeTypeClaim> RecipeTypeClaims { get; set; } = new List<RecipeTypeClaim>();
}