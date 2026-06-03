namespace AppointmentsAPI.Application.Models;

public class AppointmentResultPDF
{
    public string PatientFullName { get; set; }
    public string DoctorFullName { get; set; }
    public string Specialization { get; set; }
    public DateTime Date { get; set; }

    public string Complaints { get; set; }
    public string Conclusion { get; set; }
    public string Recommendations { get; set; }
}