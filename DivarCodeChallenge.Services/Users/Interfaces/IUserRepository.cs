using DivarCodeChallenge.Domain.Users;

namespace DivarCodeChallenge.Application.Users.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);

    Task<User?> GetByUsernameAsync(string username);

    Task<List<User>> GetAllAsync();

    Task AddAsync(User user);

    Task UpdateAsync(User user);
}
