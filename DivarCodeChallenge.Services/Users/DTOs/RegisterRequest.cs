namespace DivarCodeChallenge.Application.Users.DTOs;

public sealed class RegisterRequest
{
    public string Username { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string ConfirmPassword { get; set; } = default!;

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string NationalCode { get; set; } = default!;
}
