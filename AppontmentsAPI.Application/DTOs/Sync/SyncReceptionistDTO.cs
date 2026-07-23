namespace AppointmentsAPI.Application.DTOs.Sync;

public record SyncReceptionistDTO(
    Guid Id,
    string FirstName,
    string LastName,
    string? MiddleName
);
