namespace AppointmentsAPI.Application.DTOs;

public record SyncDoctorDTO(
    Guid Id,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Specialization
);
