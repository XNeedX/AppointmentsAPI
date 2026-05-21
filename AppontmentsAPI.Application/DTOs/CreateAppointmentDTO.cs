namespace AppointmentsAPI.Application.DTOs;

public sealed record CreateAppointmentDTO(
    Guid DoctorId,
    Guid ServiceId,
    Guid OfficeId,
    DateTime Date,
    DateTime TimeSlot
);
