using AlertaCiudadanaAPI.DTOs;
using AlertaCiudadanaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlertaCiudadanaAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService authService;

    public AuthController(AuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { mensaje = "Correo y password son requeridos." });

        var result = await authService.LoginAsync(request);
        if (result == null)
            return Unauthorized(new { mensaje = "Credenciales incorrectas." });

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest(new { mensaje = "Refresh token requerido." });

        var result = await authService.RefreshTokenAsync(request.RefreshToken);
        if (result == null)
            return Unauthorized(new { mensaje = "Token invalido o expirado." });

        return Ok(result);
    }
}
