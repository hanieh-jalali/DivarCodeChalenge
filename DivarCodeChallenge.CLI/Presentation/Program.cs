using DivarCodeChallenge.Application.Users.Services;
using DivarCodeChallenge.Infrastructure.Repositories;
using DivarCodeChallenge.Infrastructure.Security;
using DivarCodeChallenge.Infrastructure.Seed;
using DivarCodeChallenge.Presentation.Presentation.CLI;

var userRepository = new FileUserRepository();

await DataSeeder.SeedAgencyAsync(userRepository);

var passwordHasher = new PasswordHasher();

var authService = new AuthenticationService(
    userRepository,
    passwordHasher);

var menu = new MainMenu(authService);

await menu.Run();
