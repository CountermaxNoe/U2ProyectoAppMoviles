namespace AlertaCiudadanaAPI.Models;

public class Incidente
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public double? Latitud { get; set; }
    public double? Longitud { get; set; }
    public string Estado { get; set; } = "Pendiente"; // Pendiente, En proceso, Atendido
    public DateTime FechaReporte { get; set; } = DateTime.UtcNow;
    public DateTime? FechaAtencion { get; set; }

    public ICollection<FotoIncidente> Fotos { get; set; } = new List<FotoIncidente>();
}
