using DivarCodeChallenge.Domain.Contracts;
using DivarCodeChallenge.Domain.Wallets.ValueObjects;
using System.Text.Json.Serialization;

public class RentContract : Contract
{
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; private set; }

    [JsonPropertyName("endDate")]
    public DateTime EndDate { get; private set; }

    // Constructor for deserialization
    [JsonConstructor]
    public RentContract(
        Guid id,
        Guid houseId,
        Guid ownerId,
        Guid customerId,
        DateTime contractDate,
        Money amount,
        string status,
        string description,
        DateTime startDate,
        DateTime endDate)
        : base(id, houseId, ownerId, customerId,
               contractDate, amount, status, description)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    // Constructor for application usage
    public RentContract(
        Guid houseId,
        Guid ownerId,
        Guid tenantId,
        DateTime startDate,
        DateTime endDate,
        Money monthlyRent,
        string description)
        : base(houseId, ownerId, tenantId, monthlyRent, description)
    {
        StartDate = startDate;
        EndDate = endDate;
    }
}