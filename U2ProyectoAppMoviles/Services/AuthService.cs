using System.Net.Http.Json;
using U2ProyectoAppMoviles.Models;

namespace U2ProyectoAppMoviles.Services;

public class AuthService
{
    private readonly HttpClient cliente;

    public AuthService(HttpClient cliente)
    {
        this.cliente = cliente;
    }

    public async Task GuardarSesionAsync(AuthResponse resp)
    {
        await SecureStorage.SetAsync("access_token", resp.AccessToken);
        await SecureStorage.SetAsync("refresh_token", resp.RefreshToken);
        Preferences.Set("correo", resp.Correo);
        Preferences.Set("nombre_real", resp.NombreReal);
        Preferences.Set("rol", resp.Rol);

        cliente.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", resp.AccessToken);
    }

    public async Task<string?> GetAccessTokenAsync()
        => await SecureStorage.GetAsync("access_token");

    public async Task<string?> GetRefreshTokenAsync()
        => await SecureStorage.GetAsync("refresh_token");

    public string GetNombreReal() => Preferences.Get("nombre_real", "Usuario");
    public string GetCorreo() => Preferences.Get("correo", "");
    public string GetRol() => Preferences.Get("rol", "usuario");

    public async Task<bool> EstaAutenticadoAsync()
    {
        var token = await SecureStorage.GetAsync("access_token");
        return !string.IsNullOrEmpty(token);
    }

    public async Task InicializarTokenAsync()
    {
        var token = await SecureStorage.GetAsync("access_token");
        if (!string.IsNullOrEmpty(token))
        {
            cliente.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<AuthResponse?> LoginAsync(string correo, string password)
    {
        try
        {
            var body = new { Correo = correo, Password = password };
            var response = await cliente.PostAsJsonAsync("auth/login", body);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<AuthResponse?> RefreshAsync()
    {
        try
        {
            var refreshToken = await GetRefreshTokenAsync();
            if (string.IsNullOrEmpty(refreshToken)) return null;

            var body = new { RefreshToken = refreshToken };
            var response = await cliente.PostAsJsonAsync("auth/refresh", body);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }
        catch
        {
            return null;
        }
    }

    public void CerrarSesion()
    {
        SecureStorage.Remove("access_token");
        SecureStorage.Remove("refresh_token");
        Preferences.Remove("correo");
        Preferences.Remove("nombre_real");
        Preferences.Remove("rol");
        cliente.DefaultRequestHeaders.Authorization = null;
    }
}
