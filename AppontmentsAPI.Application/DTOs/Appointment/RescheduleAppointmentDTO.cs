namespace AppointmentsAPI.Application.DTOs.Appointment;

public sealed record RescheduleAppointmentDTO(Guid DoctorId, DateTime Date, DateTime TimeSlot);
