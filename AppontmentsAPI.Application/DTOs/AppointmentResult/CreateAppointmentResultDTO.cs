namespace AppointmentsAPI.Application.DTOs.AppointmentResult;

public sealed record CreateAppointmentResultDTO(
    string Complaints,
    string Conclusion,
    string Recommendations
);
