using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.App.Views;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using Grocery.Core.Services;

namespace Grocery.App.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly GlobalViewModel _global;

    [ObservableProperty] private string name;

    [ObservableProperty] private string email;

    [ObservableProperty] private string password;

    [ObservableProperty] private string confirmPassword;

    [ObservableProperty] private string registerMessage;

    public RegisterViewModel(IAuthService authService, GlobalViewModel global)
    {
        //_authService = App.Services.GetServices<IAuthService>().FirstOrDefault();
        _authService = authService;
        _global = global;
    }

    [RelayCommand]
    private async Task Register()
    {
        try
        {
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
            bool success = _authService.Register(newClient);
            if (success)
            {
                Device.BeginInvokeOnMainThread(() => 
                { 
                    Application.Current.MainPage = 
                        new LoginView(new LoginViewModel(_authService, _global)
                        {
                             LoginMessage = $"Account succesvol aangemaakt! Je kunt nu inloggen.",
                             Email = newClient.EmailAddress,
                             Password = Password
                        }
                        ); 
                });
                
                /* //Automatisch inloggen:
                var testLogin = _authService.Login(Email, Password);
                
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
                Device.BeginInvokeOnMainThread(() => { Application.Current.MainPage = new AppShell(); });
                //Einde automatisch inloggen
                */
            }
            else
            {
                RegisterMessage = "Registratie mislukt.";
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