using AppointmentsAPI.Application.Results;
using AppointmentsAPI.Domain.Models;
using System.Linq.Expressions;

namespace AppointmentsAPI.Application.Abstractions.Repositories;

public interface IAppointmentRepository
{
    Task<Result> SaveSchedulingChangeAsync(Func<Task<Result>> change, Error concurrencyError, CancellationToken cancellationToken);
    Task<Appointment> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    void Delete(Appointment appointment, CancellationToken cancellationToken = default);
    void Update(Appointment appointment, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> FindByFilterAsync(
        Expression<Func<Appointment, bool>> expression,
        Func<IQueryable<Appointment>, IOrderedQueryable<Appointment>>? orderBy = null,
        CancellationToken cancellationToken = default, 
        params Expression<Func<Appointment, object>>[] includes);
}
