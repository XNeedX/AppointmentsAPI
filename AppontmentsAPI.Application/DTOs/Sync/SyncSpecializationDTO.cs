using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Application.DTOs.Sync;

public record SyncSpecializationDTO(
    Guid Id,
    string Name,
    Status Status
);
