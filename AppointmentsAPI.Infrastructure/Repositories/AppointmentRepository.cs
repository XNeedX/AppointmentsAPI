using AppointmentsAPI.Application.Abstractions.Repositories;
using AppointmentsAPI.Application.Results;
using AppointmentsAPI.Domain.Models;
using AppointmentsAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using System.Linq.Expressions;

namespace AppointmentsAPI.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppointmentsDbContext _context;

    public AppointmentRepository(AppointmentsDbContext context)
    {
        _context = context;
    }

    public async Task<Result> SaveSchedulingChangeAsync(
        Func<Task<Result>> change, Error concurrencyError, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);

        try
        {
            var result = await change();
            if (result.IsFailure) return result;

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.SerializationFailure })
        {
            return Result.Failure(concurrencyError);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "IX_AppointmentResults_AppointmentId" })
        {
            return Result.Failure(concurrencyError);
        }
        catch (PostgresException exception) when (exception.SqlState == PostgresErrorCodes.SerializationFailure)
        {
            return Result.Failure(concurrencyError);
        }
    }

    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default) 
        => await _context.Appointments.AddAsync(appointment, cancellationToken);

    public void Delete(Appointment appointment, CancellationToken cancellationToken = default) 
        => _context.Appointments.Remove(appointment);

    public void Update(Appointment appointment, CancellationToken cancellationToken = default)
    => _context.Appointments.Update(appointment);

    public async Task<IEnumerable<Appointment>> FindByFilterAsync(
            Expression<Func<Appointment, bool>> expression,
            Func<IQueryable<Appointment>, IOrderedQueryable<Appointment>>? orderBy = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<Appointment, object>>[] includes)
    {
        IQueryable<Appointment> query = _context.Appointments.Where(expression).AsNoTracking();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync(cancellationToken);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken = default) 
        => await _context.Appointments.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
