using System.Linq.Expressions;

namespace AppointmentsAPI.Application.Abstractions;

public interface IRepository<T, K> where T : class
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindByFilterAsync(Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(K id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task Delete(T entity, CancellationToken cancellationToken = default);
}
