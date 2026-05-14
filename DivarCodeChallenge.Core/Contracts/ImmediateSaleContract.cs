using DivarCodeChallenge.Domain.Wallets;

namespace DivarCodeChallenge.Domain.Contracts;

public sealed class ImmediateSaleContract : BuyContract
{
    private ImmediateSaleContract()
    {
    }

    public ImmediateSaleContract(
        Guid houseId,
        Guid ownerId,
        Guid customerId,
        Money housePrice,
        string description)
        : base(
            houseId,
            ownerId,
            customerId,
            housePrice,
            description)
    {
    }
}
