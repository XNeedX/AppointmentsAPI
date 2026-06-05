using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Application.DTOs.Sync;

public sealed record SyncServiceDTO(Guid Id, string Name, ServiceCategory Status);