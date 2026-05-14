using DivarCodeChallenge.Application.Users.Services;
using DivarCodeChallenge.Presentation.CLI;

namespace DivarCodeChallenge.Presentation.Presentation.CLI;

public class MainMenu
{
    private readonly AuthenticationService _authService;

    public MainMenu(AuthenticationService authService)
    {
        _authService = authService;
    }

    public async Task Run()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("=== Divar ===");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("0. Exit");

            Console.Write("Select: ");

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await AuthMenu.Register(_authService);
                    break;

                case "2":
                    await AuthMenu.Login(_authService);
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Invalid option");
                    ConsoleHelper.Pause();
                    break;
            }
        }
    }
}
