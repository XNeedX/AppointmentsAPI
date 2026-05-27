using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Application.DTOs;

public record SyncSpecializationStatusUpdateDTO(
    Guid Id,
    Status Status
);