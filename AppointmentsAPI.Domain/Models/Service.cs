using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Domain.Models;

public class Service
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Status Status { get; set; } = Status.Active;
    public ServiceCategory Category { get; set; }
}
