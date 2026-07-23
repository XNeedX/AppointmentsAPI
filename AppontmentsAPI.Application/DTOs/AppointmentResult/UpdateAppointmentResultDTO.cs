namespace AppointmentsAPI.Application.DTOs.AppointmentResult;

public sealed record UpdateAppointmentResultDTO(
    string Complaints,
    string Conclusion,
    string Recommendations
);
