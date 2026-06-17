using System.Net.Http.Json;
using U2ProyectoAppMoviles.Models;

namespace U2ProyectoAppMoviles.Services;

public class IncidenteService
{
    private readonly HttpClient cliente;

    public IncidenteService(HttpClient cliente)
    {
        this.cliente = cliente;
    }

    public async Task<List<IncidenteDto>> GetTodosAsync()
    {
        try
        {
            var lista = await cliente.GetFromJsonAsync<List<IncidenteDto>>("incidentes");
            return lista ?? new List<IncidenteDto>();
        }
        catch
        {
            return new List<IncidenteDto>();
        }
    }

    public async Task<IncidenteDto?> GetByIdAsync(int id)
    {
        try
        {
            return await cliente.GetFromJsonAsync<IncidenteDto>($"incidentes/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<IncidenteDto?> CrearAsync(
        string titulo,
        string? descripcion,
        string categoria,
        string? direccion,
        double? latitud,
        double? longitud,
        string? fotoBase64)
    {
        try
        {
            var body = new
            {
                Titulo = titulo,
                Descripcion = descripcion,
                Categoria = categoria,
                Direccion = direccion,
                Latitud = latitud,
                Longitud = longitud,
                FotoBase64 = fotoBase64
            };

            var response = await cliente.PostAsJsonAsync("incidentes", body);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<IncidenteDto>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> MarcarAtendidoAsync(int id)
    {
        try
        {
            var response = await cliente.PutAsJsonAsync($"incidentes/{id}/atender", new { });
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(string? rutaLocal, string? base64)> TomarFotoAsync()
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported) return (null, null);

            var foto = await MediaPicker.Default.CapturePhotoAsync();
            if (foto == null) return (null, null);

            return await ProcesarFotoAsync(foto);
        }
        catch
        {
            return (null, null);
        }
    }

    public async Task<(string? rutaLocal, string? base64)> SeleccionarFotoAsync()
    {
        try
        {
            var foto = await MediaPicker.Default.PickPhotoAsync();
            if (foto == null) return (null, null);

            return await ProcesarFotoAsync(foto);
        }
        catch
        {
            return (null, null);
        }
    }

    private async Task<(string rutaLocal, string base64)> ProcesarFotoAsync(FileResult foto)
    {
        var rutaLocal = Path.Combine(FileSystem.CacheDirectory, foto.FileName);

        using var stream = await foto.OpenReadAsync();
        using var fileStream = File.OpenWrite(rutaLocal);
        await stream.CopyToAsync(fileStream);
        fileStream.Close();

        var bytes = await File.ReadAllBytesAsync(rutaLocal);
        var base64 = Convert.ToBase64String(bytes);

        return (rutaLocal, base64);
    }
}
