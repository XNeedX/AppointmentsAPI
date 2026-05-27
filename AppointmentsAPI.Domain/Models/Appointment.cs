namespace AppointmentsAPI.Domain.Models;

public class Appointment
{
    public Guid Id { get; set; }

    public Guid ServiceId { get; set; }
    public Service Service { get; set; }

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; }

    public Guid OfficeId { get; set; }
    public Office Office { get; set; }

    public Guid PatientId { get; set; }
    public Patient Patient { get; set; }

    public Guid ReceptionistId { get; set; }
    public Receptionist Receptionist { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;
    public DateTime TimeSlot { get; set; }
    public AppointmentResult? Result { get; set; }
}
