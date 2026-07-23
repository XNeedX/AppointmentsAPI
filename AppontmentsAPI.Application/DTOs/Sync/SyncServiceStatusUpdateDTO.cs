using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Application.DTOs.Sync;

public record SyncStatusUpdateDTO(Guid Id, Status Status);
