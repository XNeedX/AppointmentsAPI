using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Application.Results;

namespace AppointmentsAPI.Application.Abstractions;

public interface IAppointmentService
{
    Task<Result> CreateAppointmentAsync(
        CreateAppointmentDTO dto, 
        Guid patientId, 
        CancellationToken cancellationToken = default);
}
