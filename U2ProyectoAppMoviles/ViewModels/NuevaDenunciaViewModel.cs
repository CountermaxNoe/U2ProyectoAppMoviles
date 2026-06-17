using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using U2ProyectoAppMoviles.Services;

namespace U2ProyectoAppMoviles.ViewModels;

public partial class NuevaDenunciaViewModel : ObservableObject
{
    private readonly IncidenteService incidenteService;
    private readonly ConnectivityService conectividad;

    [ObservableProperty]
    private string titulo = string.Empty;

    [ObservableProperty]
    private string? descripcion;

    [ObservableProperty]
    private string categoria = string.Empty;

    [ObservableProperty]
    private string? direccion;

    [ObservableProperty]
    private string? rutaFotoLocal;

    [ObservableProperty]
    private bool tieneFoto = false;

    [ObservableProperty]
    private string mensajeError = string.Empty;

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool isEnabled = true;

    [ObservableProperty]
    private bool mostrarSnackbar;

    [ObservableProperty]
    private string textoSnackbar = string.Empty;

    private string? _fotoBase64;
    private double? _latitud;
    private double? _longitud;

    public NuevaDenunciaViewModel(
        IncidenteService incidenteService,
        ConnectivityService conectividad)
    {
        this.incidenteService = incidenteService;
        this.conectividad = conectividad;
    }

    [RelayCommand]
    public async Task TomarFotoAsync()
    {
        var (ruta, base64) = await incidenteService.TomarFotoAsync();

        if (ruta != null)
        {
            RutaFotoLocal = ruta;
            _fotoBase64 = base64;
            TieneFoto = true;
        }
        else
        {
            await MostrarSnackbarAsync("No se pudo tomar la foto.");
        }
    }

    [RelayCommand]
    public async Task SeleccionarFotoAsync()
    {
        var (ruta, base64) = await incidenteService.SeleccionarFotoAsync();

        if (ruta != null)
        {
            RutaFotoLocal = ruta;
            _fotoBase64 = base64;
            TieneFoto = true;
        }
        else
        {
            await MostrarSnackbarAsync("No se pudo seleccionar la foto.");
        }
    }

    [RelayCommand]
    public async Task ObtenerUbicacionAsync()
    {
        try
        {
            var ubicacion = await Geolocation.Default.GetLastKnownLocationAsync();

            if (ubicacion == null)
            {
                ubicacion = await Geolocation.Default.GetLocationAsync(
                    new GeolocationRequest(GeolocationAccuracy.Medium));
            }

            if (ubicacion != null)
            {
                _latitud = ubicacion.Latitude;
                _longitud = ubicacion.Longitude;

                Direccion = $"Lat: {ubicacion.Latitude:F5}, Lng: {ubicacion.Longitude:F5}";

                await MostrarSnackbarAsync("Ubicación obtenida correctamente.");
            }
        }
        catch
        {
            await MostrarSnackbarAsync("No se pudo obtener la ubicación.");
        }
    }

    [RelayCommand]
    public async Task EnviarReporteAsync()
    {
        if (string.IsNullOrWhiteSpace(Titulo))
        {
            MensajeError = "El titulo es obligatorio.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Categoria))
        {
            MensajeError = "Selecciona una categoria.";
            return;
        }

        if (!await conectividad.HayConexion())
        {
            await MostrarSnackbarAsync("Sin conexión a internet. Intenta cuando tengas red.");
            return;
        }

        IsLoading = true;
        IsEnabled = false;
        MensajeError = string.Empty;

        try
        {
            var result = await incidenteService.CrearAsync(
                Titulo,
                Descripcion,
                Categoria,
                Direccion,
                _latitud,
                _longitud,
                _fotoBase64);

            if (result != null)
            {
                await MostrarSnackbarAsync("Reporte enviado correctamente.");

                LimpiarFormulario();

                await Shell.Current.GoToAsync("..");
            }
            else
            {
                MensajeError = "No se pudo enviar el reporte. Intenta de nuevo.";
            }
        }
        catch
        {
            MensajeError = "Error al enviar. Verifica tu conexion.";
        }
        finally
        {
            IsLoading = false;
            IsEnabled = true;
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
    public async Task CancelarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void LimpiarFormulario()
    {
        Titulo = string.Empty;
        Descripcion = null;
        Categoria = string.Empty;
        Direccion = null;
        RutaFotoLocal = null;
        TieneFoto = false;
        _fotoBase64 = null;
        _latitud = null;
        _longitud = null;
    }
}