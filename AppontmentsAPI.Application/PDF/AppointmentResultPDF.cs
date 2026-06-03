using AppointmentsAPI.Application.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AppointmentsAPI.Application.Documents;

public class AppointmentResultDocument : IDocument
{
    private readonly AppointmentResultPDF _model;

    public AppointmentResultDocument(AppointmentResultPDF model)
    {
        _model = model;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(50);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);

                page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Arial));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
    }

    void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().AlignCenter().Text("InnoClinic")
                .FontSize(26).Bold().FontColor(Colors.Blue.Darken3);

            column.Item().PaddingTop(5).PaddingBottom(15)
                .LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            column.Item().Row(row =>
            {
                row.RelativeItem().Column(innerColumn =>
                {
                    innerColumn.Item().Text("Medical Report")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);

                    innerColumn.Item().Text($"Date: {_model.Date:dd.MM.yyyy HH:mm}");
                });
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(1, Unit.Centimetre).Column(column =>
        {
            column.Spacing(10);

            column.Item().Text(text =>
            {
                text.Span("Patient: ").SemiBold();
                text.Span(_model.PatientFullName);
            });

            column.Item().Text(text =>
            {
                text.Span("Attending Doctor: ").SemiBold();
                text.Span($"{_model.DoctorFullName} ({_model.Specialization})");
            });

            column.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            column.Item().Text("Complaints:").SemiBold().FontSize(14).FontColor(Colors.Grey.Darken3);
            column.Item().Text(_model.Complaints);

            column.Item().Text("Conclusion:").SemiBold().FontSize(14).FontColor(Colors.Grey.Darken3);
            column.Item().Text(_model.Conclusion);

            column.Item().Text("Recommendations:").SemiBold().FontSize(14).FontColor(Colors.Grey.Darken3);
            column.Item().Text(_model.Recommendations);
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(x =>
        {
            x.Span("Page ");
            x.CurrentPageNumber();
            x.Span(" of ");
            x.TotalPages();
        });
    }
}