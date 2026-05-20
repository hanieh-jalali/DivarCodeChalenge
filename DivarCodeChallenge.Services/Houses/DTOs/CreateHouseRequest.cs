using DivarCodeChallenge.Domain.Houses;
using DivarCodeChallenge.Domain.Houses.ValueObjects;

namespace DivarCodeChallenge.Application.Houses.DTOs;

public class CreateHouseRequest
{
    public Guid OwnerId { get; set; }

    public decimal AreaSquareMeters { get; set; }

    public int RegionCode { get; set; }

    public int Bedrooms { get; set; }

    public int Bathrooms { get; set; }

    public string HouseType { get; set; } = default!;

    public string TradeType { get; set; } = default!;

    public int? Floor { get; set; }

    public int? TerraceArea { get; set; }

    public int? YardArea { get; set; }

    public int? Floors { get; set; }
}
