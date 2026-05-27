using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Application.DTOs;

public record SyncSpecializationDTO(
    Guid Id,
    string Name,
    Status Status
);
