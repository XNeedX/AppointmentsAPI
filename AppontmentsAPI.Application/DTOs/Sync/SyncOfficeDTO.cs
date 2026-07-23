using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Application.DTOs.Sync;

public record SyncOfficeDTO(Guid Id, string Address, Status Status);

