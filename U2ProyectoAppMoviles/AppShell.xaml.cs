using U2ProyectoAppMoviles.Views;

namespace U2ProyectoAppMoviles;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        // Rutas para navegacion con GoToAsync
        Routing.RegisterRoute(nameof(NuevaDenunciaPage), typeof(NuevaDenunciaPage));
        Routing.RegisterRoute(nameof(DetalleIncidentePage), typeof(DetalleIncidentePage));
    }
}
