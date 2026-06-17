using AlertaCiudadanaAPI.Data;
using AlertaCiudadanaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AlertaCiudadanaAPI.Repositories;

public class UsuarioRepository : Repository<Usuario>
{
    public UsuarioRepository(AppDbContext context) : base(context) { }

    public async Task<Usuario?> GetByCorreoAsync(string correo)
        => await dbSet.FirstOrDefaultAsync(u => u.Correo == correo);

    public async Task<Usuario?> GetByRefreshTokenAsync(string refreshToken)
        => await dbSet.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
}
