using DivarCodeChallenge.Domain.Users;
using DivarCodeChallenge.Domain.Users.ValueObjects;

namespace DivarCodeChallenge.Application.Users.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByRoleAsync(UserRole role);
    Task<User?> GetByUsernameAsync(string username);

    Task<List<User>> GetAllAsync();

    Task AddAsync(User user);

    Task UpdateAsync(User user);
}
