namespace AppointmentsAPI.Application.DTOs;

public record ViewAppointmentListDTO(
    Guid Id,
    DateTime StartTime,
    DateTime EndTime,
    string DoctorFullName,
    string PatientFullName,
    string PatientPhoneNumber,
    string ServiceName,
    bool IsApproved   
);
