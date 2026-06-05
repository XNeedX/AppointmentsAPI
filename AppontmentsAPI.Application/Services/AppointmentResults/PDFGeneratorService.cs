using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using AppointmentsAPI.Application.Models;
using AppointmentsAPI.Application.Documents;
using AppointmentsAPI.Application.Abstractions.AppointmentResults;

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