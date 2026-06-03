namespace AppointmentsAPI.Application.DTOs;

public sealed record ViewAppointmentHistoryDTO(
    Guid AppointmentId,
    DateTime Date,
    DateTime StartTime,
    DateTime EndTime,
    string DoctorFullName,
    string ServiceName
);
