using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using U2ProyectoAppMoviles.Services;

namespace U2ProyectoAppMoviles.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService authService;
    private readonly ConnectivityService conectividad;

    [ObservableProperty]
    private string correo = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool isEnabled = true;

    public LoginViewModel(AuthService authService, ConnectivityService conectividad)
    {
        this.authService = authService;
        this.conectividad = conectividad;
    }

    [RelayCommand]
    public async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Correo) ||
            string.IsNullOrWhiteSpace(Password))
        {
            await MostrarSnackbarAsync("Ingresa tu correo y contraseña");
            return;
        }

        IsLoading = true;
        IsEnabled = false;

        try
        {
            if (!await conectividad.HayConexion())
            {
                await MostrarSnackbarAsync("Sin conexión a internet");
                return;
            }

            var result = await authService.LoginAsync(Correo, Password);

            if (result != null)
            {
                await authService.GuardarSesionAsync(result);

                Correo = string.Empty;
                Password = string.Empty;

                await Shell.Current.GoToAsync("//home");
            }
            else
            {
                await MostrarSnackbarAsync("Correo o contraseña incorrectos");
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
}