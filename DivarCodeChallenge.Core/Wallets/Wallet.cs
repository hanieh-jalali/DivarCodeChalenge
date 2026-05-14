using DivarCodeChallenge.Domain.Shared;

namespace DivarCodeChallenge.Domain.Wallets;

public sealed class Wallet : AggregateRoot
{
    private readonly List<Transaction> _transactions = new();

    public Guid UserId { get; private set; }

    public Money Balance { get; private set; }

    public IReadOnlyCollection<Transaction> Transactions =>
        _transactions.AsReadOnly();

    private Wallet()
    {
    }

    public Wallet(Guid userId)
    {
        Id = Guid.NewGuid();

        UserId = userId;

        Balance = Money.Zero;

        CreatedDate = DateTime.UtcNow;
    }

    public void Deposit(
        Money amount,
        string description)
    {
        if (amount.Amount <= 0)
            throw new InvalidOperationException(
                "Deposit amount must be greater than zero.");

        Balance += amount;

        var transaction = new Transaction(
            amount,
            TransactionTypes.Deposit,
            description);

        _transactions.Add(transaction);

        ModifiedDate = DateTime.UtcNow;
    }

    public void Withdraw(
        Money amount,
        string description)
    {
        if (amount.Amount <= 0)
            throw new InvalidOperationException(
                "Withdraw amount must be greater than zero.");

        if (Balance.Amount < amount.Amount)
            throw new InvalidOperationException(
                "Insufficient balance.");

        Balance -= amount;

        var transaction = new Transaction(
            amount,
            TransactionTypes.Withdraw,
            description);

        _transactions.Add(transaction);

        ModifiedDate = DateTime.UtcNow;
    }
}
