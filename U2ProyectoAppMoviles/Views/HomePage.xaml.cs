namespace U2ProyectoAppMoviles.Views;

using U2ProyectoAppMoviles.ViewModels;
public partial class HomePage : ContentPage
{
    private readonly HomeViewModel vm;
    public HomePage(HomeViewModel vm)
	{
		InitializeComponent();
        this.vm = vm;
        BindingContext = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        vm.CargarPerfil();
        await vm.CargarIncidentesAsync();
    }
}