using AppointmentsAPI.Application.Abstractions.AppointmentResults;
using AppointmentsAPI.Application.Documents;
using AppointmentsAPI.Application.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace AppointmentsAPI.Application.Services.AppointmentResults;

public class PDFGeneratorService : IPDFGeneratorService
{
    public PDFGeneratorService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateAppointmentResultPdf(AppointmentResultPDF model)
    {
        var document = new AppointmentResultDocument(model);

        return document.GeneratePdf();
    }
}