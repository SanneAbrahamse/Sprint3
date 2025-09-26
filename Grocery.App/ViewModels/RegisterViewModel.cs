using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using Grocery.Core.Services;

namespace Grocery.App.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly GlobalViewModel _global;
    private readonly SecureStorageService _secureStorage;

    [ObservableProperty] private string name;

    [ObservableProperty] private string email;

    [ObservableProperty] private string password;

    [ObservableProperty] private string confirmPassword;

    [ObservableProperty] private string registerMessage;

    public RegisterViewModel(IAuthService authService, GlobalViewModel global, SecureStorageService secureStorage)
    {
        //_authService = App.Services.GetServices<IAuthService>().FirstOrDefault();
        _authService = authService;
        _global = global;
        _secureStorage = secureStorage;
    }

    [RelayCommand]
    private async Task Register()
    {
        try
        {
            Console.WriteLine("=== REGISTER START ===");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Email: {Email}");

            if (Password != ConfirmPassword)
            {
                RegisterMessage = "Wachtwoorden komen niet overeen.";
                return;
            }

            var newClient = new Client(
                id: 0,
                name: Name,
                emailAddress: Email,
                password: Password
            );

            Console.WriteLine("About to call _authService.Register");
            bool success = _authService.Register(newClient);
            Console.WriteLine($"Register success: {success}");

            if (success)
            {
                Console.WriteLine("Entering success block");
                RegisterMessage = $"Account succesvol aangemaakt! Welkom {Name}";
                Console.WriteLine("RegisterMessage set");

                // SecureStorage tijdelijk overslaan vanwege macOS entitlement issue
                try 
                {
                    Console.WriteLine("About to save login");
                    await _secureStorage.SaveLoginAsync(Email, Password);
                    Console.WriteLine("Login saved");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SecureStorage failed (but continuing): {ex.Message}");
                }
                
                Console.WriteLine("About to test login");
                var testLogin = _authService.Login(Email, Password);
                Console.WriteLine($"Login test result: {(testLogin != null ? "SUCCESS" : "FAILED")}");

                if (testLogin != null)
                {
                    Console.WriteLine($"Logged in user: {testLogin.Name}");
                    _global.Client = testLogin;
                }
                else
                {
                    Console.WriteLine("Login failed, using new client object");
                    _global.Client = new Client(0, Name, Email, Password);
                }

                Console.WriteLine("About to navigate to AppShell");
                Device.BeginInvokeOnMainThread(() => { Application.Current.MainPage = new AppShell(); });
                Console.WriteLine("Navigation completed");
            }
            else
            {
                RegisterMessage = "Registratie mislukt. Misschien bestaat het account al?";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EXCEPTION in Register: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            RegisterMessage = $"Er is een fout opgetreden: {ex.Message}";
        }
    }
}