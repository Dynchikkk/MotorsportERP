namespace MotorsportErp.Domain.BaseEntities;

public abstract class BaseEntity<TId>
{
    public required TId Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; } = false;
}