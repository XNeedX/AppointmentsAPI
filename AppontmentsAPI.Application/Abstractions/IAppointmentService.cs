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
    Task<Result<ViewAppointmentResultDTO>> ViewAppointmentResultAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default);
    Task<Result> ApproveAppointmentAsync(
        Guid appointmentId, 
        CancellationToken cancellationToken = default);
    Task<Result> DeleteAppointmentAsync(
        Guid appointmentId, 
        CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeSpan>> GetAvailableTimeSlotsAsync(
        Guid doctorId,
        Guid serviceId,
        DateTime date,
        CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DoctorScheduleDTO>>> GetDoctorScheduleAsync(
        Guid doctorId, 
        DateTime date, 
        CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ViewAppointmentListDTO>>> GetFilteredAppointmentsAsync(
    GetAppointmentsFilterDTO filter,
    CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ViewAppointmentHistoryDTO>>> GetAppointmentHistoryAsync(
    Guid patientId,
    CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<PatientAppointmentHistoryDTO>>> GetPatientAppointmentHistoryAsync(
    Guid patientId,
    CancellationToken cancellationToken = default);
    Task<Result> UpdateAppointmentResultAsync(
    Guid resultId,
    UpdateAppointmentResultDTO dto,
    CancellationToken cancellationToken = default);
}
