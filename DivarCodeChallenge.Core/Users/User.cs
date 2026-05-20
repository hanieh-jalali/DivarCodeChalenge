using DivarCodeChallenge.Domain.Shared;
using DivarCodeChallenge.Domain.Users.ValueObjects;

namespace DivarCodeChallenge.Domain.Users;

public class User : AggregateRoot
{
    public string Username { get; private set; }

    public string PasswordHash { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string NationalCode { get; private set; }

    public UserRole Role { get; private set; }

    public bool IsActive { get; private set; }

    private User() { }

    public User(
        string username,
        string passwordHash,
        string firstName,
        string lastName,
        string nationalCode,
        UserRole role)
    {
        Validate(
            username,
            passwordHash,
            firstName,
            lastName,
            nationalCode);

        Id = Guid.NewGuid();

        Username = username;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        NationalCode = nationalCode;
        Role = role;

        IsActive = true;
    }

    public void Restore(
        Guid id,
        bool isActive)
    {
        Id = id;
        IsActive = isActive;
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password is required");

        PasswordHash = newPasswordHash;
    }

    public void ChangeRole(UserRole role)
    {
        Role = role;
    }

    public void UpdateProfile(
        string firstName,
        string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required");

        FirstName = firstName;
        LastName = lastName;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void Validate(
        string username,
        string passwordHash,
        string firstName,
        string lastName,
        string nationalCode)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required");

        if (string.IsNullOrWhiteSpace(nationalCode))
            throw new ArgumentException("National code is required");
    }
}
