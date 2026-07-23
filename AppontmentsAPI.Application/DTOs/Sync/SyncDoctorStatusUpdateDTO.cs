using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Application.DTOs.Sync;

public record SyncDoctorStatusUpdateDTO(
    Guid Id,
    Status Status
);
