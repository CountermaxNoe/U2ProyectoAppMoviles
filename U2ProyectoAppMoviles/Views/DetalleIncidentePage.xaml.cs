using U2ProyectoAppMoviles.ViewModels;

namespace U2ProyectoAppMoviles.Views;

public partial class DetalleIncidentePage : ContentPage
{
    private readonly DetalleIncidenteViewModel vm;
    public DetalleIncidentePage(DetalleIncidenteViewModel vm)
	{
		InitializeComponent();
        this.vm = vm;
        BindingContext = vm;
    }
}