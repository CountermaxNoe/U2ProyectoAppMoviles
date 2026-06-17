using AlertaCiudadanaAPI.DTOs;
using AlertaCiudadanaAPI.Models;
using AlertaCiudadanaAPI.Repositories;

namespace AlertaCiudadanaAPI.Services;

public class IncidenteService
{
    private readonly IncidenteRepository incidenteRepository;
    private readonly UsuarioRepository usuarioRepository;
    private readonly Repository<FotoIncidente> fotoRepository;

    public IncidenteService(
        IncidenteRepository incidenteRepo,
        UsuarioRepository usuarioRepo,
        Repository<FotoIncidente> fotoRepo)
    {
        incidenteRepository = incidenteRepo;
        usuarioRepository = usuarioRepo;
        fotoRepository = fotoRepo;
    }

    public async Task<IEnumerable<IncidenteDto>> GetTodosAsync(string baseUrl)
    {
        var incidentes = await incidenteRepository.GetAllConDetallesAsync();
        return incidentes.Select(i => MapearDto(i, baseUrl));
    }

    public async Task<IncidenteDto?> GetByIdAsync(int id, string baseUrl)
    {
        var incidente = await incidenteRepository.GetByIdConDetallesAsync(id);
        if (incidente == null) return null;
        return MapearDto(incidente, baseUrl);
    }

    public async Task<IncidenteDto?> CrearAsync(CrearIncidenteRequest request, int usuarioId, string uploadsPath, string baseUrl)
    {
        var usuario = await usuarioRepository.GetByIdAsync(usuarioId);
        if (usuario == null) return null;

        var incidente = new Incidente
        {
            UsuarioId = usuarioId,
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            Categoria = request.Categoria,
            Direccion = request.Direccion,
            Latitud = request.Latitud,
            Longitud = request.Longitud,
            Estado = "Pendiente",
            FechaReporte = DateTime.UtcNow
        };

        await incidenteRepository.AddAsync(incidente);
        await incidenteRepository.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(request.FotoBase64))
        {
            await GuardarFotoAsync(incidente.Id, request.FotoBase64, uploadsPath);
        }

        var result = await incidenteRepository.GetByIdConDetallesAsync(incidente.Id);
        return MapearDto(result!, baseUrl);
    }

    public async Task<bool> MarcarAtendidoAsync(int id)
    {
        var incidente = await incidenteRepository.GetByIdAsync(id);
        if (incidente == null) return false;

        incidente.Estado = "Atendido";
        incidente.FechaAtencion = DateTime.UtcNow;
        await incidenteRepository.UpdateAsync(incidente);
        await incidenteRepository.SaveChangesAsync();
        return true;
    }

    private async Task GuardarFotoAsync(int incidenteId, string base64, string uploadsPath)
    {
        var foto = new FotoIncidente
        {
            IncidenteId = incidenteId,
            FechaSubida = DateTime.UtcNow
        };

        await fotoRepository.AddAsync(foto);
        await fotoRepository.SaveChangesAsync();

        var nombreArchivo = $"{foto.Id}.jpg";
        foto.NombreArchivo = nombreArchivo;
        await fotoRepository.UpdateAsync(foto);
        await fotoRepository.SaveChangesAsync();

        Directory.CreateDirectory(uploadsPath);
        var rutaArchivo = Path.Combine(uploadsPath, nombreArchivo);
        var bytes = Convert.FromBase64String(base64);
        await File.WriteAllBytesAsync(rutaArchivo, bytes);
    }

    private IncidenteDto MapearDto(Incidente i, string baseUrl) => new IncidenteDto
    {
        Id = i.Id,
        Titulo = i.Titulo,
        Descripcion = i.Descripcion,
        Categoria = i.Categoria,
        Direccion = i.Direccion,
        Latitud = i.Latitud,
        Longitud = i.Longitud,
        Estado = i.Estado,
        FechaReporte = i.FechaReporte,
        FechaAtencion = i.FechaAtencion,
        NombreUsuario = i.Usuario?.NombreReal ?? "",
        UrlFotos = i.Fotos.Select(f => $"{baseUrl}uploads/{f.NombreArchivo}").ToList()
    };
}
