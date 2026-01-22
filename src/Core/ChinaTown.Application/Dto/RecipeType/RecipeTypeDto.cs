namespace ChinaTown.Application.Dto.RecipeType;

public class RecipeTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}