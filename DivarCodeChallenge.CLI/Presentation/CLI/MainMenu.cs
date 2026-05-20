using DivarCodeChallenge.Application.Users.Services;
using DivarCodeChallenge.Presentation.CLI;
using DivarCodeChallenge.Presentation.Presentation.CLI;

public class MainMenu
{
    private readonly AuthenticationService _authService;
    private readonly HomeMenu _homeMenu;

    public MainMenu(
        AuthenticationService authService,
        HomeMenu homeMenu)
    {
        _authService = authService;
        _homeMenu = homeMenu;
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

                    var userId = await AuthMenu.Login(_authService);

                    if (userId != null)
                    {
                        await _homeMenu.Run(userId.Value);
                    }

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
