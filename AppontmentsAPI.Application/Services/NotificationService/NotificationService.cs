using AppointmentsAPI.Application.Abstractions.NotificationService;
using AppointmentsAPI.Application.DTOs.AppointmentResult;
using AppointmentsAPI.Application.DTOs.NotificationService;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace AppointmentsAPI.Application.Services.NotificationService;

public class NotificationService(IConfiguration configuration) : INotificationService
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

        await SendAsync(message, cancellationToken);
    }

    public async Task SendReminderNotificationAsync(SendReminderNotificationDTO dto, CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("InnoClinic", "results@innoclinic.com"));
        message.To.Add(new MailboxAddress(dto.PatientFullName, dto.ToEmail));
        message.Subject = "Appointment Reminder";

        var bodyBuilder = new BodyBuilder
        {
            TextBody = $"Hello, {dto.PatientFullName}!\n\nThis is a reminder for your appointment on {dto.Date.ToShortDateString()} at {dto.Time.ToShortTimeString()} for {dto.ServiceName} with Dr. {dto.DoctorFullName}."
        };

        message.Body = bodyBuilder.ToMessageBody();

        await SendAsync(message, cancellationToken);
    }

    private async Task SendAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        var host = configuration["Smtp:Host"] ?? "sandbox.smtp.mailtrap.io";
        var port = configuration.GetValue("Smtp:Port", 2525);
        var useStartTls = configuration.GetValue("Smtp:UseStartTls", true);
        var username = configuration["Smtp:Username"] ?? configuration["MailTrap:Username"];
        using var client = new SmtpClient();

        await client.ConnectAsync(host, port, useStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None, cancellationToken);
        if (!string.IsNullOrWhiteSpace(username))
        {
            var password = configuration["Smtp:Password"] ?? configuration["MailTrap:Password"]
                ?? throw new InvalidOperationException("The SMTP password is required when a username is configured.");
            await client.AuthenticateAsync(username, password, cancellationToken);
        }

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
