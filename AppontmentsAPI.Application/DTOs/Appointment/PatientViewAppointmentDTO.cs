namespace AppointmentsAPI.Application.DTOs.Appointment;

public sealed record PatientViewAppointmentResultDTO(
    Guid AppointmentId,
    DateTime Date,
    string PatientFullName,
    DateTime PatientDateOfBirth,
    string DoctorFullName,
    string Specialization,
    string ServiceName,
    string Complaints,
    string Conclusion,
    string Diagnosis, 
    string Recommendations
);