namespace AppointmentsAPI.Application.DTOs;

public record PatientAppointmentHistoryDTO(
    Guid AppointmentId, 
    DateTime Date,
    DateTime StartTime,
    DateTime EndTime,
    string DoctorFullName,
    string ServiceName,
    Guid? ResultId 
);
