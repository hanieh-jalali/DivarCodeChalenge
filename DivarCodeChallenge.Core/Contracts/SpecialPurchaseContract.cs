using DivarCodeChallenge.Domain.Contracts;
using DivarCodeChallenge.Domain.Wallets.ValueObjects;
using System.Text.Json.Serialization;

public class SpecialPurchaseContract : Contract
{
    [JsonConstructor]
    public SpecialPurchaseContract(
        Guid id,
        Guid houseId,
        Guid ownerId,
        Guid customerId,
        DateTime contractDate,
        Money amount,
        string status,
        string description)
        : base(id, houseId, ownerId, customerId,
               contractDate, amount, status, description)
    {
    }

    public SpecialPurchaseContract(
        Guid houseId,
        Guid ownerId,
        Guid buyerId,
        Money price,
        string description)
        : base(houseId, ownerId, buyerId, price, description)
    {
    }
}