using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Application.DTOs;

public record SyncOfficeDTO(Guid Id, string Address, Status Status);

