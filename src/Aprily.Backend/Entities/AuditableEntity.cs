namespace Aprily.Backend.Entities;

public abstract class AuditableEntity
{
    public int Id { get; set; }
    public Guid EntityId { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;
}