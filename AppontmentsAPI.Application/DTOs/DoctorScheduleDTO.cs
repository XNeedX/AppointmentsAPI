namespace AppointmentsAPI.Application.DTOs;

public sealed record DoctorScheduleDTO(
    Guid AppointmentId,
    Guid PatientId,
    string PatientFullName,
    string ServiceName,
    DateTime StartTime,
    DateTime EndTime,   
    bool IsApproved,    
    bool HasResult
);
