using DivarCodeChallenge.Application.Contracts.Services;
using DivarCodeChallenge.Application.Houses.DTOs;
using DivarCodeChallenge.Application.Houses.Services;
using DivarCodeChallenge.Application.Wallet.Services;
using DivarCodeChallenge.Domain.Contracts;
using DivarCodeChallenge.Domain.Houses;

namespace DivarCodeChallenge.Presentation.CLI;

public class HomeMenu
{
    private readonly WalletService _walletService;
    private readonly HouseService _houseService;
    private readonly ContractService _contractService;

    public HomeMenu(
        WalletService walletService,
        HouseService houseService,
        ContractService contractService)
    {
        _walletService = walletService;
        _houseService = houseService;
        _contractService = contractService;
    }

    public async Task Run(Guid userId)
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("=== Home ===");
            Console.WriteLine("1. View Balance");
            Console.WriteLine("2. My Purchased Houses");
            Console.WriteLine("3. My Rented Houses");
            Console.WriteLine("4. Houses For Sale");
            Console.WriteLine("5. Houses For Rent");
            Console.WriteLine("6. Register House For Sale");
            Console.WriteLine("7. Register House For Rent");
            Console.WriteLine("8. Instant Sell House");
            Console.WriteLine("9. Change House State");
            Console.WriteLine("10. My Sale Contracts/cancle contract");
            Console.WriteLine("11. My Rent Contracts/cancle contract");
            Console.WriteLine("0. Logout");

            Console.Write("Select: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await ShowBalance(userId);
                    break;

                case "2":
                    await ShowPurchasedHouses(userId);
                    break;

                case "3":
                    await ShowRentedHouses(userId);
                    break;

                case "4":
                    await ShowHousesForSale(userId);
                    break;

                case "5":
                    await ShowHousesForRent(userId);
                    break;

                case "6":
                    await RegisterHouse(userId, HouseTradeType.ForSale);
                    break;

                case "7":
                    await RegisterHouse(userId, HouseTradeType.ForRent);
                    break;

                case "8":
                    await InstantSellHouse(userId);
                    break;

                case "9":
                    await ChangeHouseState(userId);
                    break;

                case "10":
                    await ShowSaleContracts(userId);
                    break;

                case "11":
                    await ShowRentContracts(userId);
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    ConsoleHelper.Pause();
                    break;
            }
        }
    }
    private async Task ShowSaleContracts(Guid userId)
    {
        Console.Clear();

        var contracts =
            await _contractService.GetUserContractsAsync(userId);

        var saleContracts = contracts
            .Where(c =>
                c.OwnerId == userId &&
                (
                    c is BuyContract ||
                    c is ImmediateSaleContract ||
                    c is SpecialPurchaseContract
                ))
            .ToList();

        if (!saleContracts.Any())
        {
            Console.WriteLine("No sale contracts found.");
            ConsoleHelper.Pause();
            return;
        }

        Console.WriteLine("=== Your Sale Contracts ===");

        foreach (var contract in saleContracts)
        {
            Console.WriteLine(
                $"ID: {contract.Id} | Amount: {contract.Amount.Amount} | Status: {contract.Status}");
        }

        await HandleCancelContract(saleContracts);
    }

    private async Task ShowRentContracts(Guid userId)
    {
        Console.Clear();

        var contracts =
            await _contractService.GetUserContractsAsync(userId);

        var rentContracts = contracts
            .Where(c =>
                c is RentContract &&
                c.CustomerId == userId)
            .ToList();

        if (!rentContracts.Any())
        {
            Console.WriteLine("No rent contracts found.");
            ConsoleHelper.Pause();
            return;
        }

        Console.WriteLine("=== Your Rent Contracts ===");

        foreach (var contract in rentContracts)
        {
            Console.WriteLine(
                $"ID: {contract.Id} | Amount: {contract.Amount.Amount} | Status: {contract.Status}");
        }

        await HandleCancelContract(rentContracts);
    }

    private async Task HandleCancelContract(List<Contract> contracts)
    {
        Console.WriteLine();
        Console.Write("Enter Contract ID to cancel (or press Enter to return): ");

        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            return;

        if (!Guid.TryParse(input, out var contractId))
        {
            Console.WriteLine("Invalid ID.");
            ConsoleHelper.Pause();
            return;
        }

        var selected = contracts.FirstOrDefault(c => c.Id == contractId);

        if (selected == null)
        {
            Console.WriteLine("Contract not found.");
            ConsoleHelper.Pause();
            return;
        }

        if (selected.Status != ContractStatuses.Active)
        {
            Console.WriteLine("Only active contracts can be cancelled.");
            ConsoleHelper.Pause();
            return;
        }

        Console.Write("Are you sure you want to cancel this contract? (y/n): ");

        if (Console.ReadLine()?.Trim().ToLower() != "y")
            return;

        try
        {
            await _contractService.CancelContractAsync(contractId);

            Console.WriteLine("Contract cancelled successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        ConsoleHelper.Pause();
    }

    private async Task ShowBalance(Guid userId)
    {
        Console.Clear();

        var balance =
            await _walletService.GetBalanceAsync(userId);

        Console.WriteLine($"Balance: {balance.Balance}");

        ConsoleHelper.Pause();
    }

    private async Task ShowPurchasedHouses(Guid userId)
    {
        Console.Clear();

        var houses = await _houseService.GetAllAsync();

        var list = houses
            .Where(h => h.OwnerId == userId)
            .ToList();

        ShowHouseList(list, "Purchased Houses");
    }

    private async Task ShowRentedHouses(Guid userId)
    {
        Console.Clear();

        var houses = await _houseService.GetAllAsync();

        var list = houses
            .Where(h => h.TenantId == userId)
            .ToList();

        ShowHouseList(list, "Rented Houses");
    }

    private async Task ShowHousesForSale(Guid userId)
    {
        Console.Clear();

        var houses =
            await _houseService.GetHousesForSaleAsync();

        if (!houses.Any())
        {
            Console.WriteLine("No houses for sale.");
            ConsoleHelper.Pause();
            return;
        }

        Console.WriteLine("=== Houses For Sale ===");

        foreach (var house in houses)
        {
            Console.WriteLine(
                $"ID: {house.Id} | Price: {house.CalculatePrice()} | Area: {house.Area.SquareMeters}");
        }

        var selectedHouse = SelectHouse(houses);

        if (selectedHouse == null)
            return;

        ShowHouseDetails(selectedHouse);

        Console.Write("Buy this house? (y/n): ");

        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            try
            {
                await _houseService.BuyHouseAsync(
                    selectedHouse.Id,
                    userId);

                Console.WriteLine("House purchased successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ConsoleHelper.Pause();
        }
    }

    private async Task ShowHousesForRent(Guid userId)
    {
        Console.Clear();

        var houses =
            await _houseService.GetHousesForRentAsync();

        if (!houses.Any())
        {
            Console.WriteLine("No houses for rent.");
            ConsoleHelper.Pause();
            return;
        }

        Console.WriteLine("=== Houses For Rent ===");

        foreach (var house in houses)
        {
            Console.WriteLine(
                $"ID: {house.Id} | Monthly Rent: {house.CalculateMonthlyRent(0.01m)}");
        }

        var selectedHouse = SelectHouse(houses);

        if (selectedHouse == null)
            return;

        ShowHouseDetails(selectedHouse);

        Console.Write("Rent this house? (y/n): ");

        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            try
            {
                await _houseService.RentHouseAsync(
                      selectedHouse.Id,
                      userId,
                      DateTime.Today,
                      DateTime.Today.AddMonths(1),
                      null);


                Console.WriteLine("House rented successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ConsoleHelper.Pause();
        }
    }

    private async Task RegisterHouse(Guid userId, string tradeType)
    {
        Console.Clear();

        try
        {
            Console.Write("House Type (Apartment/Villa/Penthouse): ");
            var houseType = Console.ReadLine();

            Console.Write("Area: ");
            var area = decimal.Parse(Console.ReadLine()!);

            Console.Write("Region Code: ");
            var regionCode = int.Parse(Console.ReadLine()!);

            Console.Write("Bedrooms: ");
            var bedrooms = int.Parse(Console.ReadLine()!);

            Console.Write("Bathrooms: ");
            var bathrooms = int.Parse(Console.ReadLine()!);

            var request = new CreateHouseRequest
            {
                OwnerId = userId,
                HouseType = houseType!,
                AreaSquareMeters = area,
                RegionCode = regionCode,
                Bedrooms = bedrooms,
                Bathrooms = bathrooms
            };

            switch (houseType!.Trim().ToLower())
            {
                case "apartment":
                    Console.Write("Floor: ");
                    request.Floor = int.Parse(Console.ReadLine()!);
                    break;

                case "penthouse":
                    Console.Write("Floor: ");
                    request.Floor = int.Parse(Console.ReadLine()!);

                    Console.Write("Terrace Area: ");
                    request.TerraceArea = int.Parse(Console.ReadLine()!);
                    break;

                case "villa":
                    Console.Write("Yard Area: ");
                    request.YardArea = int.Parse(Console.ReadLine()!);

                    Console.Write("Floors: ");
                    request.Floors = int.Parse(Console.ReadLine()!);
                    break;
            }

            var house =
                await _houseService.RegisterHouseAsync(
                    request,
                    tradeType);

            Console.WriteLine();
            Console.WriteLine("House registered successfully.");
            Console.WriteLine($"House ID: {house.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        ConsoleHelper.Pause();
    }

    private async Task InstantSellHouse(Guid userId)
    {
        Console.Clear();

        var houses =
            await _houseService.GetAllAsync();

        var ownedHouses = houses
            .Where(h => h.OwnerId == userId)
            .ToList();

        if (!ownedHouses.Any())
        {
            Console.WriteLine("No owned houses found.");
            ConsoleHelper.Pause();
            return;
        }

        Console.WriteLine("=== Your Houses ===");

        foreach (var house in ownedHouses)
        {
            Console.WriteLine(
                $"ID: {house.Id} | Price: {house.CalculatePrice()}");
        }

        var selectedHouse = SelectHouse(ownedHouses);

        if (selectedHouse == null)
            return;

        try
        {
            await _houseService.InstantSellToSystemAsync(
                selectedHouse.Id,
                userId);

            Console.WriteLine(
                "House sold instantly with 10% discount.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        ConsoleHelper.Pause();
    }

    private async Task ChangeHouseState(Guid userId)
    {
        Console.Clear();

        var housesToRent =
            await _houseService.GetHousesForRentAsync();

        Console.WriteLine("=== Houses For Rent ===");

        foreach (var house in housesToRent)
        {
            Console.WriteLine($"ID: {house.Id} | Monthly Rent: {house.CalculateMonthlyRent(0.01m)}");
        }

        var housesToSell =
           await _houseService.GetHousesForSaleAsync();

        Console.WriteLine("=== Houses For Sale ===");

        foreach (var house in housesToSell)
        {
            Console.WriteLine(
                $"ID: {house.Id} | Price: {house.CalculatePrice()} | Area: {house.Area.SquareMeters}");
        }

        var housesNotSpecified =
           await _houseService.GetHousesNotSpecifiedAsync();

        Console.WriteLine("=== Houses Not Specified ===");

        foreach (var house in housesNotSpecified)
        {
            Console.WriteLine(
                $"ID: {house.Id} | Price: {house.CalculatePrice()} | Area: {house.Area.SquareMeters}");
        }

        if (!housesToSell.Any() && !housesToRent.Any() && !housesNotSpecified.Any())
        {
            Console.WriteLine("No houses.");
            ConsoleHelper.Pause();
            return;
        }

        var selectedHouse = SelectHouse(housesToRent.Union(housesToSell).Union(housesNotSpecified).ToList());

        if (selectedHouse == null)
            return;

        ShowHouseDetails(selectedHouse);

        Console.Write("Change this house state? (y/n): ");

        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            switch (selectedHouse.TradeType)
            {
                case HouseTradeType.NotSpecified:
                    {
                        Console.Write("Put the house for sale(1) or rent(2) or both(3): ");
                        var saleOrRent = Console.ReadLine();
                        if (saleOrRent == "1")
                        { 
                            await _houseService.ChangeHouseTradeTypeAsync(selectedHouse, HouseTradeType.ForSale);
                            break;
                        }
                        if (saleOrRent == "2")
                        { 
                            await _houseService.ChangeHouseTradeTypeAsync(selectedHouse, HouseTradeType.ForRent);
                            break;
                        }
                        if (saleOrRent == "3")
                        { 
                            await _houseService.ChangeHouseTradeTypeAsync(selectedHouse, HouseTradeType.ForBoth);
                            break;
                        }

                        Console.WriteLine("Invalid option.");
                        ConsoleHelper.Pause();

                        break;
                    }

                case HouseTradeType.ForSale:
                    {
                        Console.Write("Put the house for rent(1) or both(2) or not specified(3): ");
                        var saleOrRent = Console.ReadLine();
                        if (saleOrRent == "1")
                        { 
                            await _houseService.ChangeHouseTradeTypeAsync(selectedHouse, HouseTradeType.ForRent);
                            break;
                        }
                        if (saleOrRent == "2")
                        { 
                            await _houseService.ChangeHouseTradeTypeAsync(selectedHouse, HouseTradeType.ForBoth);
                            break;
                        }
                        if (saleOrRent == "3")
                        { 
                            await _houseService.ChangeHouseTradeTypeAsync(selectedHouse, HouseTradeType.NotSpecified);
                            break;
                        }

                        Console.WriteLine("Invalid option.");
                        ConsoleHelper.Pause();

                        break;
                    }

                case HouseTradeType.ForRent:
                    {
                        Console.Write("Put the house for sale(1) or both(2) or not specified(3): ");
                        var saleOrRent = Console.ReadLine();
                        if (saleOrRent == "1")
                        { 
                            await _houseService.ChangeHouseTradeTypeAsync(selectedHouse, HouseTradeType.ForSale);
                            break;
                        }
                        if (saleOrRent == "2")
                        { 
                            await _houseService.ChangeHouseTradeTypeAsync(selectedHouse, HouseTradeType.ForBoth);
                            break;
                        }
                        if (saleOrRent == "3")
                        { 
                            await _houseService.ChangeHouseTradeTypeAsync(selectedHouse, HouseTradeType.NotSpecified);
                            break;
                        }

                        Console.WriteLine("Invalid option.");
                        ConsoleHelper.Pause();

                        break;
                    }

                case HouseTradeType.ForBoth:
                    {
                        Console.Write("Put the house for sale(1) or rent(2) or not specified(3): ");
                        var saleOrRent = Console.ReadLine();
                        if (saleOrRent == "1")
                        { 
                            await _houseService.ChangeHouseTradeTypeAsync(selectedHouse, HouseTradeType.ForSale);
                            break;
                        }
                        if (saleOrRent == "2")
                        { 
                            await _houseService.ChangeHouseTradeTypeAsync(selectedHouse, HouseTradeType.ForRent);
                            break;
                        }
                        if (saleOrRent == "3")
                        { 
                            await _houseService.ChangeHouseTradeTypeAsync(selectedHouse, HouseTradeType.NotSpecified);
                            break;
                        }

                        Console.WriteLine("Invalid option.");
                        ConsoleHelper.Pause();

                        break;
                    }
                default:
                    Console.WriteLine("Invalid option.");
                    ConsoleHelper.Pause();

                    break;
            }
        }
    }

    private House? SelectHouse(List<House> houses)
    {
        Console.WriteLine();
        Console.Write("Enter House ID: ");

        var input = Console.ReadLine();

        if (!Guid.TryParse(input, out var houseId))
        {
            Console.WriteLine("Invalid ID.");
            ConsoleHelper.Pause();
            return null;
        }

        return houses.FirstOrDefault(h => h.Id == houseId);
    }

    private void ShowHouseList(List<House> houses, string title)
    {
        if (!houses.Any())
        {
            Console.WriteLine($"No {title.ToLower()} found.");
            ConsoleHelper.Pause();
            return;
        }

        Console.WriteLine($"=== {title} ===");

        foreach (var house in houses)
        {
            Console.WriteLine(
                $"ID: {house.Id} | Price: {house.CalculatePrice()}");
        }

        var houseSelected = SelectHouse(houses);

        if (houseSelected != null)
        {
            ShowHouseDetails(houseSelected);
        }
    }

    private void ShowHouseDetails(House house)
    {
        Console.WriteLine();
        Console.WriteLine("=== House Details ===");

        Console.WriteLine($"ID: {house.Id}");
        Console.WriteLine($"Owner ID: {house.OwnerId}");
        Console.WriteLine($"Area: {house.Area.SquareMeters}");
        Console.WriteLine($"Region: {house.Region}");
        Console.WriteLine($"Bedrooms: {house.Bedrooms}");
        Console.WriteLine($"Bathrooms: {house.Bathrooms}");
        Console.WriteLine($"Price: {house.CalculatePrice()}");

        switch (house)
        {
            case Apartment apartment:
                Console.WriteLine("Type: Apartment");
                Console.WriteLine($"Floor: {apartment.Floor}");
                break;

            case Penthouse penthouse:
                Console.WriteLine("Type: Penthouse");
                Console.WriteLine($"Floor: {penthouse.Floor}");
                Console.WriteLine($"Terrace Area: {penthouse.TerraceArea}");
                break;

            case Villa villa:
                Console.WriteLine("Type: Villa");
                Console.WriteLine($"Yard Area: {villa.YardArea}");
                Console.WriteLine($"Floors: {villa.Floors}");
                break;
        }

        ConsoleHelper.Pause();
    }
}
