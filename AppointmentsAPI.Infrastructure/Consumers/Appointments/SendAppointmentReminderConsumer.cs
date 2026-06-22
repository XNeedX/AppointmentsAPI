using AppointmentsAPI.Application.DTOs.NotificationService;
using AppointmentsAPI.Application.Messages;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Appointments;

public class SendAppointmentReminderConsumer : IConsumer<SendAppointmentReminderEvent>
{
    public Task Consume(ConsumeContext<SendAppointmentReminderEvent> context)
    {
        var message = context.Message;

        var dto = new SendReminderNotificationDTO
        (
            ServiceName: message.ServiceName,
            ToEmail: message.ToEmail,
            PatientFullName: message.PatientFullName,
            Time: message.TimeSlot,
            DoctorFullName: message.DoctorFullName,
            Date: message.Date
        );

        return Task.CompletedTask;
    }
}
