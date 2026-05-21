using AppointmentsAPI.Domain.Models;

namespace AppointmentsAPI.Application.Abstractions;

public interface IAppointmentRepository
{
    Task<Appointment> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
