namespace ChinaTown.Domain.Entities;

public class RecipeTypeClaim
{
    public Guid RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public Guid RecipeTypeId { get; set; }
    public RecipeType RecipeType { get; set; } = null!;
}