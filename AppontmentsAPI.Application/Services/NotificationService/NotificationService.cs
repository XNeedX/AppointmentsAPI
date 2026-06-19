using AppointmentsAPI.Application.Abstractions.NotificationService;
using AppointmentsAPI.Application.DTOs.AppointmentResult;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AppointmentsAPI.Application.Services.NotificationService;

public class NotificationService : INotificationService
{
    public async Task SendAppointmentResultAsync(SendResultEmailDTO dto, CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("InnoClinic", "results@innoclinic.com"));
        message.To.Add(new MailboxAddress(dto.PatientName, dto.ToEmail));
        message.Subject = "Appointment result";

        var bodyBuilder = new BodyBuilder
        {
            TextBody = $"Hello, {dto.PatientName}!\n\nYou can see your result in attachment"
        };

        bodyBuilder.Attachments.Add(dto.FileName, dto.PdfBytes, new ContentType("application", "pdf"));
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync("sandbox.smtp.mailtrap.io", 2525, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync("MailTrap:Username", "MailTrap:Password", cancellationToken);
        await client.SendAsync(message, cancellationToken);

        await client.DisconnectAsync(true, cancellationToken);
    }
}
