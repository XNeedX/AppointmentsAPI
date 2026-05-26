namespace AppointmentsAPI.Application.DTOs;

public record SyncReceptionistDTO(
    Guid Id,
    string FirstName,
    string LastName,
    string? MiddleName
);
