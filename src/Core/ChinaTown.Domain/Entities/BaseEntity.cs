namespace ChinaTown.Domain.Entities;

public class BaseEntity
{
    public required Guid Id { get; set; }
    public required DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public required DateTime ModifiedOn { get; set; } = DateTime.Now;
}