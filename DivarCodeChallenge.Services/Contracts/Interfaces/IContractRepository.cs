namespace DivarCodeChallenge.Domain.Contracts;

public interface IContractRepository
{
    Task AddAsync(Contract contract);

    Task<Contract?> GetByIdAsync(Guid id);

    Task<IEnumerable<Contract>> GetByUserIdAsync(Guid userId);

    Task<IEnumerable<Contract>> GetAllAsync();

    Task UpdateAsync(Contract contract);
}
