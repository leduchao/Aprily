namespace Aprily.SharedKernel;

public abstract class Entity
{
    public int Id { get; init; }
    public Guid EntityId { get; init; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    protected Entity()
    {
        EntityId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsDeleted = false;
        DeletedAt = null;
    }
}
