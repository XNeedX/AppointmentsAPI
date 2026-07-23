namespace AppointmentsAPI.Application.DTOs.Appointment;

public sealed record ViewAppointmentHistoryDTO(
    Guid AppointmentId,
    DateTime Date,
    DateTime StartTime,
    DateTime EndTime,
    string DoctorFullName,
    string ServiceName
);
