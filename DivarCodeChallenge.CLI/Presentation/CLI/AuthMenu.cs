using DivarCodeChallenge.Application.Users.DTOs;
using DivarCodeChallenge.Application.Users.Services;

namespace DivarCodeChallenge.Presentation.CLI;

public static class AuthMenu
{
    public static async Task Register(AuthenticationService authService)
    {
        Console.Clear();

        Console.WriteLine("=== Register ===");

        Console.Write("Username: ");
        var username = Console.ReadLine()!;

        Console.Write("Password: ");
        var password = Console.ReadLine()!;

        Console.Write("Confirm Password: ");
        var confirmPassword = Console.ReadLine()!;

        Console.Write("First Name: ");
        var firstName = Console.ReadLine()!;

        Console.Write("Last Name: ");
        var lastName = Console.ReadLine()!;

        Console.Write("National Code: ");
        var nationalCode = Console.ReadLine()!;

        var request = new RegisterRequest
        {
            Username = username,
            Password = password,
            ConfirmPassword = confirmPassword,
            FirstName = firstName,
            LastName = lastName,
            NationalCode = nationalCode
        };

        try
        {
            await authService.RegisterAsync(request);

            Console.WriteLine("Registration successful.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        ConsoleHelper.Pause();
    }

    public static async Task Login(AuthenticationService authService)
    {
        Console.Clear();

        Console.WriteLine("=== Login ===");

        Console.Write("Username: ");
        var username = Console.ReadLine()!;

        Console.Write("Password: ");
        var password = Console.ReadLine()!;

        var request = new LoginRequest
        {
            Username = username,
            Password = password
        };

        try
        {
            var user = await authService.LoginAsync(request);

            Console.WriteLine();
            Console.WriteLine($"Welcome {user.FirstName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        ConsoleHelper.Pause();
    }
}
