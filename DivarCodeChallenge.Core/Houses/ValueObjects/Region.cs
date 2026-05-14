using DivarCodeChallenge.Domain.Shared;

namespace DivarCodeChallenge.Domain.Houses.ValueObjects;

public class Region : ValueObject
{
    public int Number { get; }

    public decimal Coefficient => Number switch
    {
        1 => 1.8m,
        2 => 1.4m,
        3 => 1.1m,
        4 => 0.8m,
        _ => throw new ArgumentException("Invalid region")
    };

    public Region(int number)
    {
        if (number < 1 || number > 4)
            throw new ArgumentException("Region must be between 1 and 4");

        Number = number;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Number;
    }
}
