using Grocery.App.ViewModels;
using Grocery.Core.Interfaces.Services;

namespace Grocery.App.Views;

public partial class LoginView : ContentPage
{
	public LoginView(LoginViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
	
	private async void RegisterButton_Clicked(object sender, EventArgs e)
	{
		var app = (App)Application.Current;
		var registerViewModel = new RegisterViewModel(app.AuthService, app.Global, app.SecureStorage);
		await Navigation.PushAsync(new RegisterView(registerViewModel));
	}
}