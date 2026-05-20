using DivarCodeChallenge.Domain.Shared;
using DivarCodeChallenge.Domain.Wallets.ValueObjects;
using System.Text.Json.Serialization;

namespace DivarCodeChallenge.Domain.Contracts;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$contractType")]
[JsonDerivedType(typeof(RentContract), "RentContract")]
[JsonDerivedType(typeof(BuyContract), "BuyContract")]
[JsonDerivedType(typeof(ImmediateSaleContract), "ImmediateSaleContract")]
[JsonDerivedType(typeof(SpecialPurchaseContract), "SpecialPurchaseContract")]
public abstract class Contract : AggregateRoot
{
    [JsonPropertyName("id")]
    public Guid Id { get; protected set; }

    [JsonPropertyName("houseId")]
    public Guid HouseId { get; protected set; }

    [JsonPropertyName("ownerId")]
    public Guid OwnerId { get; protected set; }

    [JsonPropertyName("customerId")]
    public Guid CustomerId { get; protected set; }

    [JsonPropertyName("contractDate")]
    public DateTime ContractDate { get; protected set; }

    [JsonPropertyName("amount")]
    public Money Amount { get; protected set; }

    [JsonPropertyName("status")]
    public string Status { get; protected set; } = default!;

    [JsonPropertyName("description")]
    public string Description { get; protected set; } = default!;

    protected Contract() { }

    [JsonConstructor]
    protected Contract(
      Guid id,
      Guid houseId,
      Guid ownerId,
      Guid customerId,
      DateTime contractDate,
      Money amount,
      string status,
      string description)
    {
        Id = id;
        HouseId = houseId;
        OwnerId = ownerId;
        CustomerId = customerId;
        ContractDate = contractDate;
        Amount = amount;
        Status = status;
        Description = description;
    }
    protected Contract(
    Guid houseId,
    Guid ownerId,
    Guid customerId,
    Money amount,
    string description)
    {
        Id = Guid.NewGuid();
        HouseId = houseId;
        OwnerId = ownerId;
        CustomerId = customerId;
        Amount = amount;
        Description = description;
        Status = ContractStatuses.Active;
        ContractDate = DateTime.UtcNow;
        CreatedDate = DateTime.UtcNow;
    }
    public void Cancel()
    {
        if (Status != ContractStatuses.Active)
            throw new InvalidOperationException("Only active contracts can be cancelled.");

        Status = ContractStatuses.Cancelled;
        ModifiedDate = DateTime.UtcNow;
    }
}
