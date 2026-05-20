using DivarCodeChallenge.Domain.Shared;

namespace DivarCodeChallenge.Domain.Houses.ValueObjects;

public class Area : ValueObject
{
    public decimal SquareMeters { get; }

    public Area(decimal squareMeters)
    {
        if (squareMeters <= 0)
            throw new ArgumentException("Area must be positive");

        SquareMeters = squareMeters;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return SquareMeters;
    }
}
