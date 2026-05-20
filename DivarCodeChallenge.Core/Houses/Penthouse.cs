using DivarCodeChallenge.Domain.Houses.ValueObjects;

namespace DivarCodeChallenge.Domain.Houses;

public class Penthouse : House
{
    private const decimal LuxuryCoefficient = 1.5m;
    private const decimal TerracePricePerMeter = 5_000_000m;

    public int TerraceArea { get; private set; }

    public int Floor { get; private set; }

    public Penthouse(
        Guid ownerId,
        Area area,
        Region region,
        int bedrooms,
        int bathrooms,
        int terraceArea,
        int floor,
        string tradeType)
        : base(ownerId, area, region, bedrooms, bathrooms, tradeType)
    {
        TerraceArea = terraceArea;
        Floor = floor;
    }

    public override decimal CalculatePrice()
    {
        var basePrice = CalculateBasePrice();

        return basePrice * LuxuryCoefficient
               + (TerraceArea * TerracePricePerMeter);
    }
}
