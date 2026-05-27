using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Domain.Models;

public class Specialization
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Status Status { get; set; }
}
