using AlertaCiudadanaAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AlertaCiudadanaAPI.Repositories;

public class Repository<T> where T : class
{
    protected readonly AppDbContext context;
    protected readonly DbSet<T> dbSet;

    public Repository(AppDbContext context)
    {
        this.context = context;
        dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) => await dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await dbSet.ToListAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await dbSet.Where(predicate).ToListAsync();

    public async Task<T> AddAsync(T entity)
    {
        await dbSet.AddAsync(entity);
        return entity;
    }

    public async Task UpdateAsync(T entity) => dbSet.Update(entity);

    public async Task DeleteAsync(T entity) => dbSet.Remove(entity);

    public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();
}
