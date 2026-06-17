using AlertaCiudadanaAPI.DTOs;
using AlertaCiudadanaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AlertaCiudadanaAPI.Controllers;

[ApiController]
[Route("incidentes")]
[Authorize]
public class IncidentesController : ControllerBase
{
    private readonly IncidenteService incidenteService;
    private readonly IWebHostEnvironment env;

    public IncidentesController(IncidenteService incidenteService, IWebHostEnvironment env)
    {
        this.incidenteService = incidenteService;
        this.env = env;
    }

    [HttpGet]
    public async Task<IActionResult> GetTodos()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}/";
        var incidentes = await incidenteService.GetTodosAsync(baseUrl);
        return Ok(incidentes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}/";
        var incidente = await incidenteService.GetByIdAsync(id, baseUrl);
        if (incidente == null) return NotFound(new { mensaje = "Incidente no encontrado." });
        return Ok(incidente);
    }

    [HttpPost]
    [Authorize(Roles = "usuario")]
    public async Task<IActionResult> Crear(CrearIncidenteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Titulo))
            return BadRequest(new { mensaje = "El titulo es requerido." });

        if (string.IsNullOrWhiteSpace(request.Categoria))
            return BadRequest(new { mensaje = "La categoria es requerida." });

        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int usuarioId);
        if (usuarioId == 0) return Unauthorized();

        var uploadsPath = Path.Combine(env.WebRootPath, "uploads");
        var baseUrl = $"{Request.Scheme}://{Request.Host}/";

        var result = await incidenteService.CrearAsync(request, usuarioId, uploadsPath, baseUrl);
        if (result == null) return BadRequest(new { mensaje = "No se pudo crear el incidente." });

        return Ok(result);
    }

    [HttpPut("{id}/atender")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> MarcarAtendido(int id)
    {
        var ok = await incidenteService.MarcarAtendidoAsync(id);
        if (!ok) return NotFound(new { mensaje = "Incidente no encontrado." });
        return Ok(new { mensaje = "Incidente marcado como atendido." });
    }
}
