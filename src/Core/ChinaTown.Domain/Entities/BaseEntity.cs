namespace ChinaTown.Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
}