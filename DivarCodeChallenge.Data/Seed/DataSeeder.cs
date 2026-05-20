using DivarCodeChallenge.Application.Users.Interfaces;
using DivarCodeChallenge.Domain.Users;
using DivarCodeChallenge.Domain.Users.ValueObjects;
using DivarCodeChallenge.Infrastructure.User.Security;

namespace DivarCodeChallenge.Infrastructure.Seed;

public static class DataSeeder
{
    public static async Task SeedAgencyAsync(IUserRepository repository)
    {
        var users = await repository.GetAllAsync();

        var agencyExists = users.Any(x => x.Role == UserRole.Agency);

        if (agencyExists)
            return;

        var hasher = new PasswordHasher();
        var passwordHash = hasher.Hash("NO_LOGIN");

        var agency = new Domain.Users.User(
            username: "divar-agency",
            passwordHash: passwordHash,
            firstName: "Divar",
            lastName: "Agency",
            nationalCode: "0000000000",
            role: UserRole.Agency
        );

        await repository.AddAsync(agency);
    }
}
