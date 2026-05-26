using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Application.DTOs;

public record SyncOfficeStatusUpdateDTO(Guid Id, Status Status);
