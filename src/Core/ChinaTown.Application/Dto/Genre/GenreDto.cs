namespace ChinaTown.Application.Dto.Genre;

public class GenreDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}