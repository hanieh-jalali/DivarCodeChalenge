using DivarCodeChallenge.Application.Contracts.Services;
using DivarCodeChallenge.Application.Houses.Services;
using DivarCodeChallenge.Application.Users.Services;
using DivarCodeChallenge.Application.Wallet.Services;
using DivarCodeChallenge.Infrastructure.Persistence;
using DivarCodeChallenge.Infrastructure.Seed;
using DivarCodeChallenge.Infrastructure.User.Repositories;
using DivarCodeChallenge.Infrastructure.User.Security;

using DivarCodeChallenge.Presentation.CLI;

var userRepository = new FileUserRepository();

await DataSeeder.SeedAgencyAsync(userRepository);

var passwordHasher = new PasswordHasher();

var walletRepository = new FileWalletRepository();
var houseRepository = new FileHouseRepository();

var walletService = new WalletService(walletRepository);
var contractService = new ContractService(
    new FileContractRepository(),
    houseRepository,
    userRepository,
    walletService);

var houseService = new HouseService(
    houseRepository,
    walletService,
    userRepository,
    contractService);

var authService = new AuthenticationService(
    userRepository,
    passwordHasher,
    walletService);

var homeMenu = new HomeMenu(
    walletService,
    houseService,
    contractService);

var menu = new MainMenu(
    authService,
    homeMenu);

await menu.Run();
