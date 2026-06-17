namespace AlertaCiudadanaAPI.Models;

public class FotoIncidente
{
    public int Id { get; set; }
    public int IncidenteId { get; set; }
    public Incidente Incidente { get; set; } = null!;
    public string NombreArchivo { get; set; } = string.Empty;
    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
}
