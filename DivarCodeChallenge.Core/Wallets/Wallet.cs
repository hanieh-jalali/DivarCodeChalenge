using DivarCodeChallenge.Domain.Shared;
using DivarCodeChallenge.Domain.Wallets.ValueObjects;

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

    private Wallet(Guid userId, Guid id, Money balance)
    {
        Id = id;
        UserId = userId;
        Balance = balance;
        CreatedDate = DateTime.UtcNow;
    }

    public static Wallet Load(
        Guid userId,
        Guid id,
        Money balance)
    {
        return new Wallet(userId, id, balance);
    }

    public void Deposit(Money amount, string transactionType)
    {
        if (amount.Amount <= 0)
            throw new InvalidOperationException(
                "Deposit amount must be greater than zero.");

        Balance += amount;

        var transaction = new Transaction(amount, transactionType);

        _transactions.Add(transaction);

        ModifiedDate = DateTime.UtcNow;
    }

    public void Withdraw(Money amount)
    {
        if (amount.Amount <= 0)
            throw new InvalidOperationException(
                "Withdraw amount must be greater than zero.");

        if (Balance.Amount < amount.Amount)
            throw new InvalidOperationException(
                "Insufficient balance.");

        Balance -= amount;

        var transaction =
            new Transaction(amount, TransactionTypes.Withdraw);

        _transactions.Add(transaction);

        ModifiedDate = DateTime.UtcNow;
    }

    public void LoadTransaction(
        Guid id,
        Money amount,
        string type,
        DateTime createdAt)
    {
        var transaction =
            new Transaction(id, amount, type, createdAt);

        _transactions.Add(transaction);
    }
}
