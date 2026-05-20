using DivarCodeChallenge.Domain.Houses.ValueObjects;
using DivarCodeChallenge.Domain.Shared;
using System.Text.Json.Serialization;

namespace DivarCodeChallenge.Domain.Houses;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Apartment), "apartment")]
[JsonDerivedType(typeof(Penthouse), "penthouse")]
[JsonDerivedType(typeof(Villa), "villa")]
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

    public string Status { get; protected set; }

    public bool IsAvailable =>
        Status == HouseStatus.Available;

    public bool IsSold =>
        Status == HouseStatus.Sold;

    public bool IsRented =>
        Status == HouseStatus.Rented;

    public bool IsForSale =>
        TradeType == HouseTradeType.ForSale ||
        TradeType == HouseTradeType.ForBoth;

    public bool IsForRent =>
        TradeType == HouseTradeType.ForRent ||
        TradeType == HouseTradeType.ForBoth;

    public bool IsNotSpecified =>
        TradeType == HouseTradeType.NotSpecified;

    protected House(
        Guid ownerId,
        Area area,
        Region region,
        int bedrooms,
        int bathrooms,
        string tradeType)
    {
        ValidateOwner(ownerId);
        ValidateCounts(bedrooms, bathrooms);
        ValidateTradeType(tradeType);

        OwnerId = ownerId;
        Area = area;
        Region = region;
        Bedrooms = bedrooms;
        Bathrooms = bathrooms;
        TradeType = tradeType;

        Status = HouseStatus.Available;
    }

    protected decimal CalculateBasePrice()
    {
        return Area.SquareMeters *
               BasePricePerMeter *
               Region.Coefficient;
    }

    public abstract decimal CalculatePrice();

    public decimal CalculateMonthlyRent(decimal rentRate)
    {
        if (rentRate <= 0)
        {
            throw new InvalidOperationException(
                "Rent rate must be greater than zero.");
        }

        return CalculatePrice() * rentRate;
    }

    public bool CanBeSold()
    {
        return IsAvailable && IsForSale;
    }

    public bool CanBeRented()
    {
        return IsAvailable && IsForRent;
    }

    public bool NotSpecified()
    {
        return IsAvailable && IsNotSpecified;
    }

    public void ValidatePurchase(Guid buyerId, Guid ownerId)
    {
        ValidateAvailability();

        if (!IsForSale)
        {
            throw new InvalidOperationException(
                "The house is not available for sale.");
        }

        if (buyerId == Guid.Empty)
        {
            throw new InvalidOperationException(
                "Invalid buyer ID.");
        }

        if (buyerId == OwnerId)
        {
            throw new InvalidOperationException(
                "The owner cannot purchase their own house.");
        }

        if (buyerId == ownerId)
            throw new InvalidOperationException(
                "The owner cannot purchase their own house.");
    }

    public void ValidateRent(Guid tenantId)
    {
        ValidateAvailability();

        if (!IsForRent)
        {
            throw new InvalidOperationException(
                "The house is not available for rent.");
        }

        if (tenantId == Guid.Empty)
        {
            throw new InvalidOperationException(
                "Invalid tenant ID.");
        }

        if (tenantId == OwnerId)
        {
            throw new InvalidOperationException(
                "The owner cannot rent their own house.");
        }
    }

    public void ValidateAvailability()
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException(
                "The house is not available.");
        }
    }

    public void MarkAsSold(
     Guid newOwnerId,
     Guid oldOwnerId,
     string tradeTypeAfterSale)
    {
        if (IsSold)
            throw new InvalidOperationException("House already sold.");

        if (OwnerId != oldOwnerId)
            throw new InvalidOperationException("Invalid owner.");

        ValidateTradeType(tradeTypeAfterSale);

        OwnerId = newOwnerId;
        Status = HouseStatus.Sold;
        TradeType = tradeTypeAfterSale;

        TenantId = null;
    }


    public void MarkAsRented(Guid tenantId)
    {
        ValidateRent(tenantId);

        TenantId = tenantId;
        Status = HouseStatus.Rented;
        TradeType = HouseTradeType.ForRent;
    }

    public void MarkAsAvailable()
    {
        Status = HouseStatus.Available;
        TradeType = HouseTradeType.NotSpecified;
    }

    private static void ValidateOwner(Guid ownerId)
    {
        if (ownerId == Guid.Empty)
        {
            throw new InvalidOperationException(
                "Invalid owner ID.");
        }
    }

    private static void ValidateCounts(
        int bedrooms,
        int bathrooms)
    {
        if (bedrooms < 0)
        {
            throw new InvalidOperationException(
                "Number of bedrooms cannot be negative.");
        }

        if (bathrooms < 0)
        {
            throw new InvalidOperationException(
                "Number of bathrooms cannot be negative.");
        }
    }

    private static void ValidateTradeType(string tradeType)
    {
        if (string.IsNullOrWhiteSpace(tradeType))
        {
            throw new InvalidOperationException(
                "Trade type is required.");
        }

        var validTradeTypes = new[]
        {
            HouseTradeType.NotSpecified,
            HouseTradeType.ForSale,
            HouseTradeType.ForRent,
            HouseTradeType.ForBoth
        };

        if (!validTradeTypes.Contains(tradeType))
        {
            throw new InvalidOperationException(
                "Invalid trade type.");
        }
    }

    public void ChangeTradeType(string tradeType)
    {
        ValidateTradeType(tradeType);

        TradeType = tradeType;
    }
}
