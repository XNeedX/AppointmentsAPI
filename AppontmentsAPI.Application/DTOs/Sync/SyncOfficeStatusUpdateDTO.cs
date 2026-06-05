using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Application.DTOs.Sync;

public record SyncOfficeStatusUpdateDTO(Guid Id, Status Status);
