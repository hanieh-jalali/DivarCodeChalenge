using DivarCodeChallenge.Domain.Contracts;
using DivarCodeChallenge.Domain.Wallets.ValueObjects;
using System.Text.Json.Serialization;

public class BuyContract : Contract
{
    [JsonPropertyName("housePrice")]
    public Money HousePrice { get; private set; }

    [JsonConstructor]
    public BuyContract(
        Guid id,
        Guid houseId,
        Guid ownerId,
        Guid customerId,
        DateTime contractDate,
        Money amount,
        string status,
        string description,
        Money housePrice)
        : base(id, houseId, ownerId, customerId,
               contractDate, amount, status, description)
    {
        HousePrice = housePrice;
    }

    public BuyContract(
        Guid houseId,
        Guid ownerId,
        Guid customerId,
        Money price,
        string description)
        : base(houseId, ownerId, customerId, price, description)
    {
        HousePrice = price;
    }
}