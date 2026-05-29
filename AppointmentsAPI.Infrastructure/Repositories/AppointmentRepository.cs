using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Domain.Models;
using AppointmentsAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppointmentsAPI.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppointmentsDbContext _context;

    public AppointmentRepository(AppointmentsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default) 
        => await _context.Appointments.AddAsync(appointment, cancellationToken);

    public void Delete(Appointment appointment, CancellationToken cancellationToken = default) 
        => _context.Appointments.Remove(appointment);

    public async Task<IEnumerable<Appointment>> FindByFilterAsync(
        Expression<Func<Appointment, bool>> expression, 
        CancellationToken cancellationToken = default, 
        params Expression<Func<Appointment, object>>[] includes)
    {
        IQueryable<Appointment> query = _context.Appointments.Where(expression).AsNoTracking();

        foreach (var include in includes)
        {
            query = query.Include(include);
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
