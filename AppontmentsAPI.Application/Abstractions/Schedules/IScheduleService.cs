using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Application.DTOs.Appointment;
using AppointmentsAPI.Application.DTOs.Schedule;
using AppointmentsAPI.Application.Results;

namespace AppointmentsAPI.Application.Abstractions.Schedules;

public interface IScheduleService
{
    Task<IEnumerable<TimeSpan>> GetAvailableTimeSlotsAsync(Guid doctorId, Guid serviceId, DateTime date, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DoctorScheduleDTO>>> GetDoctorScheduleAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ViewAppointmentHistoryDTO>>> GetAppointmentHistoryAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<PatientAppointmentHistoryDTO>>> GetPatientAppointmentHistoryAsync(Guid patientId, CancellationToken cancellationToken = default);
}