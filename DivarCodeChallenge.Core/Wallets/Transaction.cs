using DivarCodeChallenge.Domain.Shared;
using DivarCodeChallenge.Domain.Wallets.ValueObjects;

namespace DivarCodeChallenge.Domain.Wallets;

public sealed class Transaction : BaseEntity
{
    public Money Amount { get; private set; }

    public string Type { get; private set; }

    public string? Description { get; private set; }

    private Transaction()
    {
    }

    public Transaction(Money amount, string type, string? description = null)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        Type = type;
        Description = description;
        CreatedDate = DateTime.UtcNow;
    }

    internal Transaction(Guid id, Money amount, string type, DateTime createdAt, string? description = null)
    {
        Id = id;
        Amount = amount;
        Type = type;
        Description = description;
        CreatedDate = createdAt;
    }
}
