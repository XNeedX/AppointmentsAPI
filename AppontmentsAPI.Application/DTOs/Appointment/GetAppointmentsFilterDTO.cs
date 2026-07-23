namespace AppointmentsAPI.Application.DTOs.Appointment;

public record GetAppointmentsFilterDTO(
    DateTime? Date,
    string? DoctorName,
    string? ServiceName,
    bool? IsApproved,
    Guid? OfficeId
);