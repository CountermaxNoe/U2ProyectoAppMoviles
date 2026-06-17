using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using U2ProyectoAppMoviles.Models;
using U2ProyectoAppMoviles.Services;

namespace U2ProyectoAppMoviles.ViewModels;

[QueryProperty(nameof(IncidenteId), "IncidenteId")]
public partial class DetalleIncidenteViewModel : ObservableObject
{
    private readonly IncidenteService incidenteService;
    private readonly AuthService authService;
    private readonly ConnectivityService conectividad;

    [ObservableProperty]
    private int incidenteId;

    [ObservableProperty]
    private IncidenteDto? incidente;

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private string mensajeError = string.Empty;

    [ObservableProperty]
    private bool esAdmin = false;

    [ObservableProperty]
    private bool puedeAtender = false;

    public DetalleIncidenteViewModel(
        IncidenteService incidenteService,
        AuthService authService,
        ConnectivityService conectividad)
    {
        this.incidenteService = incidenteService;
        this.authService = authService;
        this.conectividad = conectividad;
    }

    partial void OnIncidenteIdChanged(int value)
    {
        _ = CargarDetalleAsync();
    }

    [RelayCommand]
    public async Task CargarDetalleAsync()
    {
        if (IncidenteId <= 0)
            return;

        if (!await conectividad.HayConexion())
        {
            var snackbar = Snackbar.Make(
                "Sin conexión a internet.",
                null,
                string.Empty,
                TimeSpan.FromSeconds(3));

            await snackbar.Show();
            return;
        }

        IsLoading = true;
        MensajeError = string.Empty;

        try
        {
            Incidente = await incidenteService.GetByIdAsync(IncidenteId);

            EsAdmin = authService.GetRol() == "admin";
            PuedeAtender = EsAdmin && Incidente?.Estado != "Atendido";
        }
        catch
        {
            MensajeError = "No se pudo cargar el detalle.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task MarcarAtendidoAsync()
    {
        if (!await conectividad.HayConexion())
        {
            var snackbar = Snackbar.Make(
                "Sin conexión a internet. Verifica tu red e intenta de nuevo.",
                null,
                string.Empty,
                TimeSpan.FromSeconds(3));

            await snackbar.Show();
            return;
        }

        bool confirmar = await Shell.Current.DisplayAlert(
            "Confirmar",
            "Marcar este incidente como atendido?",
            "Si",
            "No");

        if (!confirmar)
            return;

        IsLoading = true;

        try
        {
            var ok = await incidenteService.MarcarAtendidoAsync(IncidenteId);

            if (ok)
            {
                await Shell.Current.DisplayAlert(
                    "Listo",
                    "Incidente marcado como atendido.",
                    "OK");

                await CargarDetalleAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    "No se pudo marcar como atendido.",
                    "OK");
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task AbrirMapaAsync()
    {
        if (Incidente?.Latitud == null || Incidente?.Longitud == null)
        {
            await Shell.Current.DisplayAlert(
                "Aviso",
                "Este incidente no tiene coordenadas.",
                "OK");

            return;
        }

        var location = new Location(
            Incidente.Latitud.Value,
            Incidente.Longitud.Value);

        var options = new MapLaunchOptions
        {
            Name = Incidente.Titulo
        };

        await Map.Default.OpenAsync(location, options);
    }
}