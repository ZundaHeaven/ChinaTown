namespace ChinaTown.Domain.Entities;

public class Genre : BaseEntity
{
    public required string Name { get; set; }

    public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
}