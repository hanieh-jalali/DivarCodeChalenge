using DivarCodeChallenge.Domain.Houses.ValueObjects;

namespace DivarCodeChallenge.Domain.Houses;

public class Villa : House
{
    private const decimal YardPricePerMeter = 3_000_000m;
    private const decimal FloorPremium = 50_000_000m;

    public int YardArea { get; private set; }

    public int Floors { get; private set; }

    public Villa(
        Guid ownerId,
        Area area,
        Region region,
        int bedrooms,
        int bathrooms,
        int yardArea,
        int floors,
        string tradeType)
        : base(ownerId, area, region, bedrooms, bathrooms, tradeType)
    {
        YardArea = yardArea;
        Floors = floors;
    }

    public override decimal CalculatePrice()
    {
        var basePrice = CalculateBasePrice();

        return basePrice
               + (YardArea * YardPricePerMeter)
               + (Floors * FloorPremium);
    }
}
