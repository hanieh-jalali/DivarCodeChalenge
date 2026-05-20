using DivarCodeChallenge.Application.Users.Interfaces;
using DivarCodeChallenge.Domain.Users;
using DivarCodeChallenge.Domain.Users.ValueObjects;

namespace DivarCodeChallenge.Infrastructure.User.Repositories;

public sealed class FileUserRepository : IUserRepository
{
    private readonly string _filePath;

    public FileUserRepository()
    {
        _filePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Persistence",
            "Files",
            "users.txt");

        EnsureFileExists();
    }

    public async Task AddAsync(Domain.Users.User user)
    {
        var users = await GetAllAsync();

        var exists = users.Any(x =>
            x.Username.Equals(
                user.Username,
                StringComparison.OrdinalIgnoreCase));

        if (exists)
            throw new InvalidOperationException("Username already exists.");

        var line = Serialize(user);

        await File.AppendAllTextAsync(
            _filePath,
            line + Environment.NewLine);
    }

    public async Task<Domain.Users.User?> GetByIdAsync(Guid id)
    {
        var users = await GetAllAsync();

        return users.FirstOrDefault(x => x.Id == id);
    }
    
    public async Task<Domain.Users.User?> GetByRoleAsync(UserRole role)
    {
        var users = await GetAllAsync();

        return users.FirstOrDefault(x => x.Role == role);
    }

    public async Task<Domain.Users.User?> GetByUsernameAsync(string username)
    {
        var users = await GetAllAsync();

        return users.FirstOrDefault(x =>
            x.Username.Equals(
                username,
                StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<Domain.Users.User>> GetAllAsync()
    {
        var lines = await File.ReadAllLinesAsync(_filePath);

        return lines
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(Deserialize)
            .ToList();
    }

    public async Task UpdateAsync(Domain.Users.User user)
    {
        var users = await GetAllAsync();

        var index = users.FindIndex(x => x.Id == user.Id);

        if (index < 0)
            throw new InvalidOperationException("User not found.");

        users[index] = user;

        var lines = users
            .Select(Serialize)
            .ToArray();

        await File.WriteAllLinesAsync(_filePath, lines);
    }

    private string Serialize(Domain.Users.User user)
    {
        return string.Join('|',
            user.Id,
            user.Username,
            user.PasswordHash,
            user.FirstName,
            user.LastName,
            user.NationalCode,
            user.Role.Value,
            user.IsActive);
    }

    private Domain.Users.User Deserialize(string line)
    {
        var parts = line.Split('|');

        var id = Guid.Parse(parts[0]);

        var username = parts[1];

        var passwordHash = parts[2];

        var firstName = parts[3];

        var lastName = parts[4];

        var nationalCode = parts[5];

        var role = UserRole.From(parts[6]);

        var isActive = bool.Parse(parts[7]);

        var user = new Domain.Users.User(
            username,
            passwordHash,
            firstName,
            lastName,
            nationalCode,
            role);

        user.Restore(id, isActive);

        return user;
    }

    private void EnsureFileExists()
    {
        var directory = Path.GetDirectoryName(_filePath)!;

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(_filePath))
        {
            using var stream = File.Create(_filePath);
        }
    }
}
