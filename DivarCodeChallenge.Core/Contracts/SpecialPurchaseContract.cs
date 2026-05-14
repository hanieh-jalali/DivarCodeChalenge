using DivarCodeChallenge.Domain.Wallets;

namespace DivarCodeChallenge.Domain.Contracts;

public sealed class SpecialPurchaseContract : BuyContract
{
    public int InstallmentMonths { get; private set; }

    private SpecialPurchaseContract()
    {
    }

    public SpecialPurchaseContract(
        Guid houseId,
        Guid ownerId,
        Guid customerId,
        Money housePrice,
        int installmentMonths,
        string description)
        : base(
            houseId,
            ownerId,
            customerId,
            housePrice,
            description)
    {
        if (installmentMonths <= 0)
            throw new InvalidOperationException(
                "Installment months must be greater than zero.");

        InstallmentMonths = installmentMonths;
    }
}
