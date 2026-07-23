namespace AppointmentsAPI.Application.Configurations;

public class InnoClinicOptions
{
    public const string SectionName = "InnoClinicSettings";
    public string TimeZoneId { get; set; } = "UTC"; 
}