namespace ChinaTown.Application.Dto.Region;

public class RegionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public int RecipeCount { get; set; }
}