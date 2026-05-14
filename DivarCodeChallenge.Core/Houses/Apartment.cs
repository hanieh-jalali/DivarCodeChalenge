using DivarCodeChallenge.Domain.Houses.ValueObjects;

namespace DivarCodeChallenge.Domain.Houses;

public class Apartment : House
{
    public int Floor { get; private set; }

    public Apartment(
        Guid ownerId,
        Area area,
        Region region,
        int bedrooms,
        int bathrooms,
        int floor,
        string tradeType)
        : base(ownerId, area, region, bedrooms, bathrooms, tradeType)
    {
        Floor = floor;
    }

    public override decimal CalculatePrice()
    {
        var basePrice = CalculateBasePrice();

        return basePrice *
               (1 + 0.03m * Bedrooms) *
               (1 + 0.01m * Floor);
    }
}
