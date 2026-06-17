namespace AlertaCiudadanaAPI.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string NombreReal { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "usuario"; // "usuario" o "admin"
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
}
