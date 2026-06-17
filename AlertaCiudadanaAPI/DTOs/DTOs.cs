namespace AlertaCiudadanaAPI.DTOs;

public class LoginRequest
{
    public string Correo { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = null!;
}

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string NombreReal { get; set; } = null!;
    public string Rol { get; set; } = null!;
}

public class CrearIncidenteRequest
{
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string Categoria { get; set; } = null!;
    public string? Direccion { get; set; }
    public double? Latitud { get; set; }
    public double? Longitud { get; set; }
    public string? FotoBase64 { get; set; }
}

public class IncidenteDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string Categoria { get; set; } = null!;
    public string? Direccion { get; set; }
    public double? Latitud { get; set; }
    public double? Longitud { get; set; }
    public string Estado { get; set; } = null!;
    public DateTime FechaReporte { get; set; }
    public DateTime? FechaAtencion { get; set; }
    public string NombreUsuario { get; set; } = null!;
    public List<string> UrlFotos { get; set; } = new();
}

public class MarcarAtendidoRequest
{
    public int Id { get; set; }
}
