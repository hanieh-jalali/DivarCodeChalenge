using DivarCodeChallenge.Application.Wallet.DTOs;
using DivarCodeChallenge.Application.Wallet.Interface;
using DivarCodeChallenge.Domain.Shared;
using DivarCodeChallenge.Domain.Wallets;
using DivarCodeChallenge.Domain.Wallets.ValueObjects;

namespace DivarCodeChallenge.Application.Wallet.Services;

public class WalletService
{
    private readonly IWalletRepository _walletRepository;

    public WalletService(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task CreateWalletForUserAsync(Guid userId)
    {
        var existingWallet = await _walletRepository.GetByUserIdAsync(userId);

        if (existingWallet != null)
            throw new InvalidOperationException("Wallet already exists.");

        var wallet = new Domain.Wallets.Wallet(userId);

        wallet.Deposit(
            new Money(100000),
            TransactionTypes.InitialGift);

        await _walletRepository.AddAsync(wallet);
    }

    public async Task<BalanceResponse> GetBalanceAsync(Guid userId)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);

        if (wallet == null)
            throw new InvalidOperationException("Wallet not found.");

        return new BalanceResponse
        {
            Balance = wallet.Balance.Amount
        };
    }

    public async Task DepositAsync(WalletAmountRequest request)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId);

        if (wallet == null)
            throw new InvalidOperationException("Wallet not found.");

        wallet.Deposit(
            new Money(request.Amount),
            TransactionTypes.Deposit);

        await _walletRepository.UpdateAsync(wallet);
    }

    public async Task WithdrawAsync(WalletAmountRequest request)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId);

        if (wallet == null)
            throw new InvalidOperationException("Wallet not found.");

        wallet.Withdraw(new Money(request.Amount));

        await _walletRepository.UpdateAsync(wallet);
    }

    public async Task<List<TransactionResponse>> GetTransactionsAsync(Guid userId)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);

        if (wallet == null)
            throw new InvalidOperationException("Wallet not found.");

        return wallet.Transactions
            .Select(t => new TransactionResponse
            {
                Id = t.Id,
                Amount = t.Amount.Amount,
                Type = t.Type,
                CreatedAt = t.CreatedDate
            })
            .ToList();
    }
}
