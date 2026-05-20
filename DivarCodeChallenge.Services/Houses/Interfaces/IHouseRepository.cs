using DivarCodeChallenge.Domain.Houses;

namespace DivarCodeChallenge.Application.Houses.Interfaces;

public interface IHouseRepository
{
    Task<House?> GetByIdAsync(Guid id);
    Task<List<House>> GetAllAsync();
    Task AddAsync(House house);
    Task UpdateAsync(House house);
    Task DeleteAsync(Guid id);
}
