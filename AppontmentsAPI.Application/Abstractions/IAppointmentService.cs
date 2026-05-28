using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Application.Results;

namespace AppointmentsAPI.Application.Abstractions;

public interface IAppointmentService
{
    Task<Result> CreateAppointmentAsync(
        CreateAppointmentDTO dto, 
        Guid patientId, 
        CancellationToken cancellationToken = default);
    Task<Result> CreateAppointmentResultAsync(
        Guid appointmentId, 
        CreateAppointmentResultDTO dto, 
        CancellationToken cancellationToken = default);
    Task<Result> ApproveAppointmentAsync(
        Guid appointmentId, 
        CancellationToken cancellationToken = default);
    Task<Result> DeleteAppointmentAsync(
        Guid appointmentId, 
        CancellationToken cancellationToken = default);
}
