using DivarCodeChallenge.Application.Contracts.Services;
using DivarCodeChallenge.Application.Houses.DTOs;
using DivarCodeChallenge.Application.Houses.Interfaces;
using DivarCodeChallenge.Application.Users.Interfaces;
using DivarCodeChallenge.Application.Wallet.DTOs;
using DivarCodeChallenge.Application.Wallet.Services;
using DivarCodeChallenge.Domain.Houses;
using DivarCodeChallenge.Domain.Houses.ValueObjects;
using DivarCodeChallenge.Domain.Users.ValueObjects;
using DivarCodeChallenge.Domain.Wallets.ValueObjects;

namespace DivarCodeChallenge.Application.Houses.Services;

public class HouseService
{
    private readonly IHouseRepository _houseRepository;
    private readonly WalletService _walletService;
    private readonly IUserRepository _userRepository;
    private readonly ContractService _contractService;

    private const decimal DefaultRentRate = 0.004m;
    private const decimal InstantSellCoefficient = 0.9m;

    public HouseService(
        IHouseRepository houseRepository,
        WalletService walletService,
        IUserRepository userRepository,
        ContractService contractService)
    {
        _houseRepository = houseRepository;
        _walletService = walletService;
        _userRepository = userRepository;
        _contractService = contractService;
    }

    public async Task<House> RegisterHouseAsync(CreateHouseRequest request, string tradeType)
    {
        ValidateCreateHouseRequest(request);
        ValidateTradeType(tradeType);
        ValidateHouseSpecificFields(request);

        var house = CreateHouse(request, tradeType);

        await _houseRepository.AddAsync(house);

        return house;
    }

    public async Task<List<House>> GetAllAsync()
    {
        return await _houseRepository.GetAllAsync();
    }

    public async Task<List<House>> GetHousesForSaleAsync()
    {
        var houses = await _houseRepository.GetAllAsync();

        return houses
            .Where(h => h.CanBeSold())
            .ToList();
    }

    public async Task<List<House>> GetHousesForRentAsync()
    {
        var houses = await _houseRepository.GetAllAsync();

        return houses
            .Where(h => h.CanBeRented())
            .ToList();
    }

    public async Task<List<House>> GetHousesNotSpecifiedAsync()
    {
        var houses = await _houseRepository.GetAllAsync();

        return houses
            .Where(h => h.NotSpecified())
            .ToList();
    }

    public Task<House?> GetByIdAsync(Guid id)
    {
        return _houseRepository.GetByIdAsync(id);
    }

    public async Task RentHouseAsync(
       Guid houseId,
       Guid tenantId,
       DateTime startDate,
       DateTime endDate,
       decimal? rentRate = null)
    {
        if (tenantId == Guid.Empty)
            throw new InvalidOperationException("Invalid tenant id.");

        if (endDate <= startDate)
            throw new InvalidOperationException("Invalid rental period.");

        var house = await GetRequiredHouseAsync(houseId);

        house.ValidateRent(tenantId);

        var rate = rentRate ?? DefaultRentRate;

        if (rate <= 0)
            throw new InvalidOperationException("Invalid rent rate.");

        var monthlyRent = house.CalculateMonthlyRent(rate);

        var totalAmount = monthlyRent;

        await TransferMoneyAsync(tenantId, house.OwnerId, totalAmount);

        await _contractService.CreateRentAsync(
            houseId,
            house.OwnerId,
            tenantId,
            startDate,
            endDate,
            new Money(monthlyRent));

        house.MarkAsRented(tenantId);

        await _houseRepository.UpdateAsync(house);
    }


    public async Task BuyHouseAsync(Guid houseId, Guid buyerId)
    {
        if (buyerId == Guid.Empty)
            throw new InvalidOperationException("Invalid buyer id.");

        var house = await GetRequiredHouseAsync(houseId);

        house.ValidatePurchase(buyerId, house.OwnerId);

        var price = house.CalculatePrice();

        await TransferMoneyAsync(buyerId, house.OwnerId, price);

        await _contractService.CreateBuyAsync(
            houseId,
            house.OwnerId,
            buyerId,
            new Money(price));

        house.MarkAsSold(buyerId, house.OwnerId, HouseTradeType.NotSpecified);

        await _houseRepository.UpdateAsync(house);
    }

    public async Task InstantSellToSystemAsync(Guid houseId, Guid ownerId)
    {
        var house = await GetRequiredHouseAsync(houseId);

        if (house.OwnerId != ownerId)
            throw new InvalidOperationException("You are not the owner of this house.");

        house.ValidateAvailability();

        if (!house.CanBeSold())
            throw new InvalidOperationException("House is not available for sale.");

        const decimal instantSellCoefficient = 0.9m;
        var instantPrice = house.CalculatePrice() * instantSellCoefficient;

        var agencyUser = await _userRepository.GetByRoleAsync(UserRole.Agency);
        if (agencyUser == null)
            throw new InvalidOperationException("Agency user not found.");

        await _walletService.DepositAsync(new WalletAmountRequest
        {
            UserId = ownerId,
            Amount = instantPrice
        });

        await _contractService.CreateImmediateSaleAsync(
            houseId,
            ownerId,
            agencyUser.Id,
            new Money(instantPrice));

        house.MarkAsSold(
            agencyUser.Id,
            ownerId,
            HouseTradeType.ForBoth);

        await _houseRepository.UpdateAsync(house);
    }

    public async Task ChangeHouseTradeTypeAsync(House house, string tradeType) 
    {
        house.ChangeTradeType(tradeType);
        await _houseRepository.UpdateAsync(house);
    }

    private async Task<House> GetRequiredHouseAsync(Guid houseId)
    {
        return await _houseRepository.GetByIdAsync(houseId)
               ?? throw new InvalidOperationException("House not found.");
    }

    private async Task TransferMoneyAsync(Guid fromUserId, Guid toUserId, decimal amount)
    {
        if (fromUserId == Guid.Empty)
            throw new InvalidOperationException("Invalid sender id.");

        if (toUserId == Guid.Empty)
            throw new InvalidOperationException("Invalid receiver id.");

        if (amount <= 0)
            throw new InvalidOperationException("Invalid transfer amount.");

        var sender = await _userRepository.GetByIdAsync(fromUserId)
                     ?? throw new InvalidOperationException("Sender not found.");

        var receiver = await _userRepository.GetByIdAsync(toUserId)
                       ?? throw new InvalidOperationException("Receiver not found.");

        await _walletService.WithdrawAsync(new WalletAmountRequest
        {
            UserId = sender.Id,
            Amount = amount
        });

        await _walletService.DepositAsync(new WalletAmountRequest
        {
            UserId = receiver.Id,
            Amount = amount
        });
    }

    private static House CreateHouse(CreateHouseRequest request, string tradeType)
    {
        var area = new Area(request.AreaSquareMeters);
        var region = new Region(request.RegionCode);

        var type = request.HouseType.Trim().ToLower();

        return type.ToLower() switch
        {
            "apartment" =>
                new Apartment(
                    request.OwnerId,
                    area,
                    region,
                    request.Bedrooms,
                    request.Bathrooms,
                    request.Floor!.Value,
                    tradeType),

            "penthouse" =>
                new Penthouse(
                    request.OwnerId,
                    area,
                    region,
                    request.Bedrooms,
                    request.Bathrooms,
                    request.TerraceArea!.Value,
                    request.Floor!.Value,
                    tradeType),

            "villa" =>
                new Villa(
                    request.OwnerId,
                    area,
                    region,
                    request.Bedrooms,
                    request.Bathrooms,
                    request.YardArea!.Value,
                    request.Floors!.Value,
                    tradeType),

            _ => throw new InvalidOperationException("Invalid house type.")
        };
    }

    private static void ValidateCreateHouseRequest(CreateHouseRequest request)
    {
        if (request is null)
            throw new InvalidOperationException("Invalid request.");

        if (request.OwnerId == Guid.Empty)
            throw new InvalidOperationException("Invalid owner id.");

        if (string.IsNullOrWhiteSpace(request.HouseType))
            throw new InvalidOperationException("House type is required.");

        if (request.AreaSquareMeters <= 0)
            throw new InvalidOperationException("Area must be greater than zero.");

        if (request.Bedrooms < 0)
            throw new InvalidOperationException("Bedrooms cannot be negative.");

        if (request.Bathrooms < 0)
            throw new InvalidOperationException("Bathrooms cannot be negative.");
    }

    private static void ValidateHouseSpecificFields(CreateHouseRequest request)
    {
        var type = request.HouseType.Trim().ToLower();

        switch (type)
        {
            case "apartment":
                if (!request.Floor.HasValue)
                    throw new InvalidOperationException("Floor is required.");
                break;

            case "penthouse":
                if (!request.Floor.HasValue)
                    throw new InvalidOperationException("Floor is required.");

                if (!request.TerraceArea.HasValue)
                    throw new InvalidOperationException("Terrace area is required.");
                break;

            case "villa":
                if (!request.YardArea.HasValue)
                    throw new InvalidOperationException("Yard area is required.");

                if (!request.Floors.HasValue)
                    throw new InvalidOperationException("Number of floors is required.");
                break;

            default:
                throw new InvalidOperationException("Invalid house type.");
        }
    }

    private static void ValidateTradeType(string tradeType)
    {
        if (string.IsNullOrWhiteSpace(tradeType))
            throw new InvalidOperationException("Trade type is required.");

        var validTradeTypes = new[]
        {
            HouseTradeType.ForSale,
            HouseTradeType.ForRent,
            HouseTradeType.ForBoth
        };

        if (!validTradeTypes.Contains(tradeType))
            throw new InvalidOperationException("Invalid trade type.");
    }
}
