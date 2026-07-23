namespace AppointmentsAPI.Application.DTOs.Appointment;

public sealed record CreateAppointmentDTO(
    Guid DoctorId,
    Guid ServiceId,
    Guid OfficeId,
    DateTime Date,
    DateTime TimeSlot
);
