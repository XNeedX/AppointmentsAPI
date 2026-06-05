using AppointmentsAPI.Application.Models;

namespace AppointmentsAPI.Application.Abstractions.AppointmentResults;

public interface IPDFGeneratorService
{
    byte[] GenerateAppointmentResultPdf(AppointmentResultPDF model);
}