namespace DivarCodeChallenge.Domain.Shared;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedDate { get; protected set; } = DateTime.UtcNow;
    public Guid? CreatedByUser { get; protected set; }

    public DateTime? ModifiedDate { get; protected set; }
    public Guid? ModifiedByUser { get; protected set; }

    public void SetCreatedBy(Guid userId)
    {
        CreatedByUser = userId;
    }

    public void SetModifiedBy(Guid userId)
    {
        ModifiedByUser = userId;
        ModifiedDate = DateTime.UtcNow;
    }
}
