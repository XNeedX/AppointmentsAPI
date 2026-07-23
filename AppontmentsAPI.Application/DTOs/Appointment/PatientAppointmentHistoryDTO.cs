namespace AppointmentsAPI.Application.DTOs.Appointment;

public record PatientAppointmentHistoryDTO(
    Guid AppointmentId, 
    DateTime Date,
    DateTime StartTime,
    DateTime EndTime,
    string DoctorFullName,
    string ServiceName,
    Guid? ResultId 
);
