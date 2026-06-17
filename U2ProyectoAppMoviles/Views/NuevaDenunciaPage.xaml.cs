namespace U2ProyectoAppMoviles.Views;

using U2ProyectoAppMoviles.ViewModels;

public partial class NuevaDenunciaPage : ContentPage
{
    private readonly NuevaDenunciaViewModel vm;
    public NuevaDenunciaPage(NuevaDenunciaViewModel vm)
	{
		InitializeComponent();
        this.vm = vm;
        BindingContext = vm;
    }
}