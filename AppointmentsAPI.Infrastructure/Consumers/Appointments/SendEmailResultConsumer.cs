using AppointmentsAPI.Application.Abstractions.NotificationService;
using AppointmentsAPI.Application.DTOs.AppointmentResult;
using AppointmentsAPI.Application.Messages;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Appointments;

public class SendEmailResultConsumer : IConsumer<SendAppointmentResultEmailEvent>
{
    private readonly INotificationService _notificationService;

    public SendEmailResultConsumer(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Consume(ConsumeContext<SendAppointmentResultEmailEvent> context)
    {
        var msg = context.Message;

        var dto = new SendResultEmailDTO(
                   msg.ToEmail,
                   msg.PatientName,
                   msg.PdfBytes,
                   msg.FileName
        );

        await _notificationService.SendAppointmentResultAsync(dto,context.CancellationToken);
    }
}
