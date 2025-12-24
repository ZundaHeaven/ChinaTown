namespace ChinaTown.Domain.Entities;

public class Genre : BaseEntity
{
    public required string Name { get; set; }
    public virtual ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
}