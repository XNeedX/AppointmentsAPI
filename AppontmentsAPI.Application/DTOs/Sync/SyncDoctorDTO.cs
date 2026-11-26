namespace AppointmentsAPI.Application.DTOs.Sync;

using AppointmentsAPI.Domain.Enums;

public record SyncDoctorDTO(
    Guid Id,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Specialization,
    Status? Status = null
);
