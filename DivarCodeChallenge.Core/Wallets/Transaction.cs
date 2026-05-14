using DivarCodeChallenge.Domain.Shared;

namespace DivarCodeChallenge.Domain.Wallets;

public sealed class Transaction : BaseEntity
{
    public Money Amount { get; private set; }

    public string Type { get; private set; }

    public string Description { get; private set; }

    private Transaction()
    {
    }

    public Transaction(
        Money amount,
        string type,
        string description)
    {
        Id = Guid.NewGuid();

        Amount = amount;

        Type = type;

        Description = description;

        CreatedDate = DateTime.UtcNow;
    }
}
