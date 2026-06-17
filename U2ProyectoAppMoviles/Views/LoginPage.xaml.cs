namespace U2ProyectoAppMoviles.Views;

using U2ProyectoAppMoviles.ViewModels;
public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel vm;
    public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();
        this.vm = vm;
        BindingContext = vm;
    }
}