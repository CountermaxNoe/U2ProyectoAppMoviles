using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using U2ProyectoAppMoviles.Models;
using U2ProyectoAppMoviles.Services;
using U2ProyectoAppMoviles.Views;

namespace U2ProyectoAppMoviles.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IncidenteService incidenteService;
    private readonly AuthService authService;
    private readonly ConnectivityService conectividad;

    [ObservableProperty]
    private ObservableCollection<IncidenteDto> incidentes = new();

    [ObservableProperty]
    private string nombreBienvenida = string.Empty;

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private string mensajeError = string.Empty;

    [ObservableProperty]
    private bool esAdmin = false;

    public HomeViewModel(
        IncidenteService incidenteService,
        AuthService authService,
        ConnectivityService conectividad)
    {
        this.incidenteService = incidenteService;
        this.authService = authService;
        this.conectividad = conectividad;
    }

    public void CargarPerfil()
    {
        NombreBienvenida = authService.GetNombreReal();
        EsAdmin = authService.GetRol() == "admin";
    }

    [RelayCommand]
    public async Task CargarIncidentesAsync()
    {
        IsLoading = true;
        MensajeError = string.Empty;

        try
        {
            if (!await conectividad.HayConexion())
            {
                await MostrarSnackbarAsync("Sin conexión a internet");
                return;
            }

            var lista = await incidenteService.GetTodosAsync();

            Incidentes.Clear();

            foreach (var item in lista)
            {
                Incidentes.Add(item);
            }
        }
        catch (HttpRequestException ex) when (
            ex.InnerException is TimeoutException ||
            ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
        {
            await MostrarSnackbarAsync("Tiempo de espera agotado");
        }
        catch (Exception)
        {
            await MostrarSnackbarAsync("No se pudo conectar con el servidor");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task MostrarSnackbarAsync(string mensaje)
    {
        var snackbar = Snackbar.Make(
            mensaje,
            null,
            string.Empty,
            TimeSpan.FromSeconds(3));

        await snackbar.Show();
    }

    [RelayCommand]
    public async Task IrANuevaDenunciaAsync()
    {
        await Shell.Current.GoToAsync(nameof(NuevaDenunciaPage));
    }

    [RelayCommand]
    public async Task VerDetalleAsync(IncidenteDto incidente)
    {
        if (incidente == null)
            return;

        var param = new Dictionary<string, object>
        {
            { "IncidenteId", incidente.Id }
        };

        await Shell.Current.GoToAsync(nameof(DetalleIncidentePage), param);
    }

    [RelayCommand]
    public void CerrarSesion()
    {
        authService.CerrarSesion();
        Shell.Current.GoToAsync("//login");
    }
}