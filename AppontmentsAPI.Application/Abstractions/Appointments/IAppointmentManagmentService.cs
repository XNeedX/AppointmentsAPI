using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Application.DTOs.Appointment;
using AppointmentsAPI.Application.Results;

namespace AppointmentsAPI.Application.Abstractions.Appointments;

public interface IAppointmentManagementService
{
    Task<Result> CreateAppointmentAsync(CreateAppointmentDTO dto, Guid patientId, CancellationToken cancellationToken = default);
    Task<Result> ApproveAppointmentAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<Result> DeleteAppointmentAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ViewAppointmentListDTO>>> GetFilteredAppointmentsAsync(GetAppointmentsFilterDTO filter, CancellationToken cancellationToken = default);
}