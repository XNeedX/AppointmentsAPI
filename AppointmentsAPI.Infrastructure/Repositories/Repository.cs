using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppointmentsAPI.Infrastructure.Repositories;

internal class Repository<T, K> : IRepository<T, K>
    where T : class
{
    protected readonly AppointmentsDbContext _dbContext;
    protected readonly DbSet<T> _dbset;

    public Repository(AppointmentsDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbset = _dbContext.Set<T>();
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) => await _dbset.AddAsync(entity);

    public async Task<IEnumerable<T>> FindByFilterAsync(Expression<Func<T, bool>> expression, 
        CancellationToken cancellationToken = default, 
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbset.Where(expression).AsNoTracking();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default) => await _dbset.AsNoTracking().ToListAsync();

    public async Task<T?> GetByIdAsync(K id, CancellationToken cancellationToken = default) => await _dbset.FindAsync(id).AsTask();

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) => await _dbContext.SaveChangesAsync();

    public void Delete(T entity, CancellationToken cancellationToken = default) => _dbset.Remove(entity);
}