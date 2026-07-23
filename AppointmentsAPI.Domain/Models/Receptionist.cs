using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Domain.Models;

public class Receptionist
{
    public Guid Id { get; set; } 
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public Status Status { get; set; } = Status.Active;
}