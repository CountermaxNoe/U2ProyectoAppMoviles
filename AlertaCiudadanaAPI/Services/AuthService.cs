using AlertaCiudadanaAPI.DTOs;
using AlertaCiudadanaAPI.Models;
using AlertaCiudadanaAPI.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AlertaCiudadanaAPI.Services;

public class AuthService
{
    private readonly UsuarioRepository usuarioRepository;
    private readonly IConfiguration config;

    public AuthService(UsuarioRepository usuarioRepo, IConfiguration config)
    {
        usuarioRepository = usuarioRepo;
        this.config = config;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var usuario = await usuarioRepository.GetByCorreoAsync(request.Correo);
        if (usuario == null) return null;
        if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash)) return null;

        return await GenerarTokensAsync(usuario);
    }

    public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken)
    {
        var usuario = await usuarioRepository.GetByRefreshTokenAsync(refreshToken);
        if (usuario == null) return null;
        if (usuario.RefreshTokenExpiryTime < DateTime.UtcNow) return null;

        return await GenerarTokensAsync(usuario);
    }

    private async Task<AuthResponse> GenerarTokensAsync(Usuario usuario)
    {
        var accessToken = GenerarAccessToken(usuario);
        var newRefreshToken = Guid.NewGuid().ToString();

        usuario.RefreshToken = newRefreshToken;
        usuario.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await usuarioRepository.UpdateAsync(usuario);
        await usuarioRepository.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            Correo = usuario.Correo,
            NombreReal = usuario.NombreReal,
            Rol = usuario.Rol
        };
    }

    private string GenerarAccessToken(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Correo),
            new Claim("nombreReal", usuario.NombreReal),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
