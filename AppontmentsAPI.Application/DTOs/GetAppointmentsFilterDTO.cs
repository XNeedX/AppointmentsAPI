namespace AppointmentsAPI.Application.DTOs;

public record GetAppointmentsFilterDTO(
    DateTime? Date,
    string? DoctorName,
    string? ServiceName,
    bool? IsApproved,
    Guid? OfficeId
);