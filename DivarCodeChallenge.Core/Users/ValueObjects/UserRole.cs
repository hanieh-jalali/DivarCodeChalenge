using DivarCodeChallenge.Domain.Shared;

namespace DivarCodeChallenge.Domain.Users.ValueObjects;

public sealed class UserRole : ValueObject
{
    public static readonly UserRole Normal = new("Normal");
    public static readonly UserRole Agency = new("Agency");

    public string Value { get; }

    private UserRole(string value)
    {
        Value = value;
    }

    public static UserRole From(string value)
    {
        return value switch
        {
            "Normal" => Normal,
            "Agency" => Agency,
            _ => throw new ArgumentException("Invalid user role")
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
