namespace U2ProyectoAppMoviles.Models;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string NombreReal { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}

public class IncidenteDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public double? Latitud { get; set; }
    public double? Longitud { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaReporte { get; set; }
    public DateTime? FechaAtencion { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public List<string> UrlFotos { get; set; } = new();

    // Propiedades calculadas para la UI
    public string FechaRelativa
    {
        get
        {
            var diff = DateTime.UtcNow - FechaReporte;
            if (diff.TotalMinutes < 60) return $"hace {(int)diff.TotalMinutes} min";
            if (diff.TotalHours < 24) return $"hace {(int)diff.TotalHours}h";
            return $"hace {(int)diff.TotalDays}d";
        }
    }

    public string PrimeraFoto => UrlFotos.FirstOrDefault() ?? string.Empty;
    public bool TieneFoto => UrlFotos.Count > 0;
}
