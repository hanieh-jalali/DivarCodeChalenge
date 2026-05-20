namespace DivarCodeChallenge.Application.Wallet.Interface;

public interface IWalletRepository
{
    Task<Domain.Wallets.Wallet?> GetByUserIdAsync(Guid userId);
    Task AddAsync(Domain.Wallets.Wallet wallet);
    Task UpdateAsync(Domain.Wallets.Wallet wallet);
}
