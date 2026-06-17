using U2ProyectoAppMoviles.Services;
namespace U2ProyectoAppMoviles.Views;

public partial class SplashPage : ContentPage
{
    private readonly AuthService authService;
    public SplashPage(AuthService authService)
	{
		InitializeComponent();
        this.authService = authService;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(2000);

        await authService.InicializarTokenAsync();
        bool autenticado = await authService.EstaAutenticadoAsync();

        if (autenticado)
            await Shell.Current.GoToAsync("//home");
        else
            await Shell.Current.GoToAsync("//login");
    }
}