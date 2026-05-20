using DivarCodeChallenge.Application.Houses.Interfaces;
using DivarCodeChallenge.Application.Users.Interfaces;
using DivarCodeChallenge.Application.Wallet.DTOs;
using DivarCodeChallenge.Application.Wallet.Services;
using DivarCodeChallenge.Domain.Contracts;
using DivarCodeChallenge.Domain.Users.ValueObjects;
using DivarCodeChallenge.Domain.Wallets.ValueObjects;

namespace DivarCodeChallenge.Application.Contracts.Services;

public class ContractService
{
    private readonly IContractRepository _contracts;
    private readonly IHouseRepository _houses;
    private readonly IUserRepository _users;
    private readonly WalletService _walletService;

    public ContractService(
        IContractRepository contracts,
        IHouseRepository houses,
        IUserRepository users,
        WalletService walletService)
    {
        _contracts = contracts;
        _houses = houses;
        _users = users;
        _walletService = walletService; 
    }

    private async Task ValidateUserToUserAsync(Guid houseId, Guid ownerId, Guid customerId)
    {
        if (houseId == Guid.Empty)
            throw new Exception("Invalid house id.");

        if (ownerId == Guid.Empty)
            throw new Exception("Invalid owner id.");

        if (customerId == Guid.Empty)
            throw new Exception("Invalid customer id.");

        if (await _houses.GetByIdAsync(houseId) is null)
            throw new Exception("House not found.");

        if (await _users.GetByIdAsync(ownerId) is null)
            throw new Exception("Owner not found.");

        if (await _users.GetByIdAsync(customerId) is null)
            throw new Exception("Customer not found.");
    }

    public async Task<RentContract> CreateRentAsync(
        Guid houseId,
        Guid ownerId,
        Guid tenantId,
        DateTime start,
        DateTime end,
        Money rent)
    {
        await ValidateUserToUserAsync(houseId, ownerId, tenantId);

        var contract = new RentContract(
            houseId,
            ownerId,
            tenantId,
            start,
            end,
            rent,
            "House rent contract");

        await _contracts.AddAsync(contract);
        return contract;
    }

    public async Task<BuyContract> CreateBuyAsync(
        Guid houseId,
        Guid ownerId,
        Guid buyerId,
        Money price)
    {
        await ValidateUserToUserAsync(houseId, ownerId, buyerId);

        var contract = new BuyContract(
            houseId,
            ownerId,
            buyerId,
            price,
            "House buy contract");

        await _contracts.AddAsync(contract);
        return contract;
    }

    public async Task<ImmediateSaleContract> CreateImmediateSaleAsync(
        Guid houseId,
        Guid ownerId,
        Guid buyerId,
        Money price)
    {
        await ValidateUserToUserAsync(houseId, ownerId, buyerId);

        var contract = new ImmediateSaleContract(
            houseId,
            ownerId,
            buyerId,
            price,
            "Immediate sale contract");

        await _contracts.AddAsync(contract);
        return contract;
    }

    public async Task<SpecialPurchaseContract> CreateSpecialPurchaseAsync(
        Guid houseId,
        Guid ownerId,
        Guid buyerId,
        Money price)
    {
        await ValidateUserToUserAsync(houseId, ownerId, buyerId);

        var contract = new SpecialPurchaseContract(
            houseId,
            ownerId,
            buyerId,
            price,
            "Installment purchase");

        await _contracts.AddAsync(contract);
        return contract;
    }

    public Task<IEnumerable<Contract>> GetUserContractsAsync(Guid userId)
        => _contracts.GetByUserIdAsync(userId);

    public Task<Contract?> GetContractByIdAsync(Guid contractId)
        => _contracts.GetByIdAsync(contractId);

    public async Task CancelContractAsync(Guid contractId)
    {
        var contract = await _contracts.GetByIdAsync(contractId)
                       ?? throw new InvalidOperationException("Contract not found.");

        if (contract.Status != ContractStatuses.Active)
            throw new InvalidOperationException("Only active contracts can be cancelled.");

        var amount = contract.Amount;         
        var penalty = new Money(amount.Amount * 0.10m); 

        var balance = await _walletService.GetBalanceAsync(contract.CustomerId);
        if (balance.Balance < penalty.Amount)
            throw new InvalidOperationException("Insufficient balance for cancellation penalty.");

        await _walletService.WithdrawAsync(new WalletAmountRequest
        {
            UserId = contract.CustomerId,
            Amount = penalty.Amount
        });

        await _walletService.DepositAsync(new WalletAmountRequest
        {
            UserId = contract.CustomerId,
            Amount = amount.Amount
        });

        contract.Cancel();

        if (contract is BuyContract ||
            contract is ImmediateSaleContract ||
            contract is SpecialPurchaseContract)
        {
            var house = await _houses.GetByIdAsync(contract.HouseId)
                        ?? throw new InvalidOperationException("House not found.");

            house.MarkAsAvailable();

            await _houses.UpdateAsync(house);
        }

        await _contracts.UpdateAsync(contract);
    }

}
