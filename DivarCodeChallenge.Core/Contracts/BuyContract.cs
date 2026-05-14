using DivarCodeChallenge.Domain.Wallets;

namespace DivarCodeChallenge.Domain.Contracts;

public class BuyContract : Contract
{
    public Money HousePrice { get; private set; }

    protected BuyContract()
    {
    }

    public BuyContract(
        Guid houseId,
        Guid ownerId,
        Guid customerId,
        Money housePrice,
        string description)
        : base(
            houseId,
            ownerId,
            customerId,
            DateTime.UtcNow,
            DateTime.UtcNow,
            housePrice,
            description)
    {
        HousePrice = housePrice;
    }
}
