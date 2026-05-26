using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Application.DTOs;

public record SyncStatusUpdateDTO(Guid Id, Status Status);
