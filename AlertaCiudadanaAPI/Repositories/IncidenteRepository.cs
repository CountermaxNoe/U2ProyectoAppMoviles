using AlertaCiudadanaAPI.Data;
using AlertaCiudadanaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AlertaCiudadanaAPI.Repositories;

public class IncidenteRepository : Repository<Incidente>
{
    public IncidenteRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Incidente>> GetAllConDetallesAsync()
        => await dbSet
            .Include(i => i.Usuario)
            .Include(i => i.Fotos)
            .OrderByDescending(i => i.FechaReporte)
            .ToListAsync();

    public async Task<Incidente?> GetByIdConDetallesAsync(int id)
        => await dbSet
            .Include(i => i.Usuario)
            .Include(i => i.Fotos)
            .FirstOrDefaultAsync(i => i.Id == id);
}
