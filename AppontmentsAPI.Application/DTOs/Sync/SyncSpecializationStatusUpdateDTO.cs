using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Application.DTOs.Sync;

public record SyncSpecializationStatusUpdateDTO(
    Guid Id,
    Status Status
);