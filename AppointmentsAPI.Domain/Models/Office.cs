using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Domain.Models;

public class Office
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Status Status { get; set; } = Status.Active;
    public string Address { get; set; }
}
