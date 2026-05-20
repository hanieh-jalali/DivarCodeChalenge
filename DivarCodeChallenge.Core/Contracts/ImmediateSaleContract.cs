using DivarCodeChallenge.Domain.Contracts;
using DivarCodeChallenge.Domain.Wallets.ValueObjects;
using System.Text.Json.Serialization;

public class ImmediateSaleContract : Contract
{
    [JsonConstructor]
    public ImmediateSaleContract(
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

    public ImmediateSaleContract(
        Guid houseId,
        Guid ownerId,
        Guid customerId,
        Money amount,
        string description)
        : base(houseId, ownerId, customerId, amount, description)
    {
    }
}