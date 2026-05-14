using DivarCodeChallenge.Domain.Houses.ValueObjects;
using DivarCodeChallenge.Domain.Shared;

namespace DivarCodeChallenge.Domain.Houses;

public abstract class House : AggregateRoot
{
    protected const decimal BasePricePerMeter = 10_000_000m;

    public Guid OwnerId { get; protected set; }

    public Guid? TenantId { get; protected set; }

    public Area Area { get; protected set; }

    public Region Region { get; protected set; }

    public int Bedrooms { get; protected set; }

    public int Bathrooms { get; protected set; }

    public string TradeType { get; protected set; }

    protected House(
        Guid ownerId,
        Area area,
        Region region,
        int bedrooms,
        int bathrooms,
        string tradeType)
    {
        OwnerId = ownerId;
        Area = area;
        Region = region;
        Bedrooms = bedrooms;
        Bathrooms = bathrooms;
        TradeType = tradeType;
    }

    protected decimal CalculateBasePrice()
    {
        return Area.SquareMeters * BasePricePerMeter * Region.Coefficient;
    }

    public abstract decimal CalculatePrice();

    public decimal CalculateMonthlyRent(decimal rentRate)
    {
        return CalculatePrice() * rentRate;
    }

    public void AssignTenant(Guid tenantId)
    {
        TenantId = tenantId;
    }

    public void RemoveTenant()
    {
        TenantId = null;
    }
}
