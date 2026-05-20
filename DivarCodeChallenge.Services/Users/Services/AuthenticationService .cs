using DivarCodeChallenge.Application.Users.DTOs;
using DivarCodeChallenge.Application.Users.Interfaces;
using DivarCodeChallenge.Application.Wallet.Services;
using DivarCodeChallenge.Domain.Users;
using DivarCodeChallenge.Domain.Users.ValueObjects;

namespace DivarCodeChallenge.Application.Users.Services;

public sealed class AuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly WalletService _walletService;

    public AuthenticationService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        WalletService walletService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _walletService = walletService;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Username))
            throw new InvalidOperationException("Username is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidOperationException("Password is required.");

        if (request.Password != request.ConfirmPassword)
            throw new InvalidOperationException("Passwords do not match.");

        var username = request.Username.Trim();

        var existingUser =
            await _userRepository.GetByUsernameAsync(username);

        if (existingUser is not null)
            throw new InvalidOperationException("Username already exists.");

        var passwordHash =
            _passwordHasher.Hash(request.Password);

        var user = new User(
            username,
            passwordHash,
            request.FirstName,
            request.LastName,
            request.NationalCode,
            UserRole.Normal);

        await _userRepository.AddAsync(user);

        await _walletService.CreateWalletForUserAsync(user.Id);
    }

    public async Task<User> LoginAsync(LoginRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidOperationException("Invalid username or password.");

        var username = request.Username.Trim();

        var user =
            await _userRepository.GetByUsernameAsync(username);

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
