using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Application.DTOs;

public record SyncDoctorStatusUpdateDTO(
    Guid Id,
    Status Status
);
