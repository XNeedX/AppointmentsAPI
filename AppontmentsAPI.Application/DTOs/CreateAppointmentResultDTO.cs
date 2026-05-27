namespace AppointmentsAPI.Application.DTOs;

public sealed record CreateAppointmentResultDTO(
    string Complaints,
    string Conclusion,
    string Recommendations
);
