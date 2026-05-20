using DivarCodeChallenge.Domain.Shared;

namespace DivarCodeChallenge.Domain.Wallets.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }

    public static Money Zero => new(0);

    public Money(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException(
                "Amount cannot be negative.");

        Amount = amount;
    }

    public static Money operator +(Money left, Money right)
    {
        return new Money(left.Amount + right.Amount);
    }

    public static Money operator -(Money left, Money right)
    {
        return new Money(left.Amount - right.Amount);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
    }

    public override string ToString()
    {
        return Amount.ToString("N0");
    }
}
