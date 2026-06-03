namespace AppointmentsAPI.Application.DTOs;

public sealed record UpdateAppointmentResultDTO(
    string Complaints,
    string Conclusion,
    string Recommendations
);
