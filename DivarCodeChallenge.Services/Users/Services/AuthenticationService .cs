using DivarCodeChallenge.Application.Users.DTOs;
using DivarCodeChallenge.Application.Users.Interfaces;
using DivarCodeChallenge.Domain.Users;
using DivarCodeChallenge.Domain.Users.ValueObjects;

namespace DivarCodeChallenge.Application.Users.Services;

public sealed class AuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthenticationService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        if (request.Password != request.ConfirmPassword)
            throw new InvalidOperationException("Passwords do not match.");

        var existingUser =
            await _userRepository.GetByUsernameAsync(request.Username);

        if (existingUser is not null)
            throw new InvalidOperationException("Username already exists.");

        var passwordHash =
            _passwordHasher.Hash(request.Password);

        var user = new User(
            request.Username,
            passwordHash,
            request.FirstName,
            request.LastName,
            request.NationalCode,
            UserRole.Normal);

        await _userRepository.AddAsync(user);
    }

    public async Task<User> LoginAsync(LoginRequest request)
    {
        var user =
            await _userRepository.GetByUsernameAsync(request.Username);

        if (user is null)
            throw new InvalidOperationException("Invalid username or password.");

        var isValid =
            _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!isValid)
            throw new InvalidOperationException("Invalid username or password.");

        if (!user.IsActive)
            throw new InvalidOperationException("User account is inactive.");

        return user;
    }
}
