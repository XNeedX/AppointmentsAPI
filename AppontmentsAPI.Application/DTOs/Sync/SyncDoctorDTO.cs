namespace AppointmentsAPI.Application.DTOs.Sync;

public record SyncDoctorDTO(
    Guid Id,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Specialization
);
