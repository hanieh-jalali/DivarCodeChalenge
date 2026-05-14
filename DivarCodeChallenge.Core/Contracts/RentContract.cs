using DivarCodeChallenge.Domain.Wallets;

namespace DivarCodeChallenge.Domain.Contracts;

public sealed class RentContract : Contract
{
    public Money MonthlyRent { get; private set; }

    public Money DepositAmount { get; private set; }

    private RentContract()
    {
    }

    public RentContract(
        Guid houseId,
        Guid ownerId,
        Guid customerId,
        DateTime startDate,
        DateTime endDate,
        Money monthlyRent,
        Money depositAmount,
        string description)
        : base(
            houseId,
            ownerId,
            customerId,
            startDate,
            endDate,
            monthlyRent,
            description)
    {
        MonthlyRent = monthlyRent;

        DepositAmount = depositAmount;
    }
}
