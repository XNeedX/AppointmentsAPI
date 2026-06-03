namespace AppointmentsAPI.Application.DTOs;

public sealed record ViewAppointmentResultDTO(
    Guid AppointmentId,
    DateTime Date,
    string PatientFullName,
    string DoctorFullName,
    DateTime PatientDateOfBirth,
    Guid DoctorId,
    string Specialization,
    string ServiceName,
    string Complaints,
    string Conclusion,
    string Recommendations
);
