using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Application.DTOs.AppointmentResult;
using AppointmentsAPI.Application.Results;

namespace AppointmentsAPI.Application.Abstractions.AppointmentResults;

public interface IAppointmentResultService
{
    Task<Result> CreateAppointmentResultAsync(Guid appointmentId, CreateAppointmentResultDTO dto, CancellationToken cancellationToken = default);
    Task<Result> UpdateAppointmentResultAsync(Guid resultId, UpdateAppointmentResultDTO dto, CancellationToken cancellationToken = default);
    Task<Result<ViewAppointmentResultDTO>> ViewAppointmentResultAsync(Guid appointmentId, CancellationToken cancellationToken = default);
}