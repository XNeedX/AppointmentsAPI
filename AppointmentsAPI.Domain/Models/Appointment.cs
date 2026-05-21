namespace AppointmentsAPI.Domain.Models;

public class Appointment
{
    public Guid Id { get; set; }

    public Guid ServiceId { get; set; }
    public Service Service { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;
    public DateTime TimeSlot { get; set; }
}
