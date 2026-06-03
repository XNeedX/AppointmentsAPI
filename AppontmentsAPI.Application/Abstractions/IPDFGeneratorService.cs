using AppointmentsAPI.Application.Models;

namespace AppointmentsAPI.Application.Abstractions;

public interface IPDFGeneratorService
{
    byte[] GenerateAppointmentResultPdf(AppointmentResultPDF model);
}