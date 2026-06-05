namespace AppointmentsAPI.Application.DTOs.Sync;

public record SyncPatientDTO(
    Guid Id,
    string? AccountId,
    string FirstName,
    string LastName,
    string? MiddleName,
    string PhoneNumber,
    DateTime DateOfBirth
);
