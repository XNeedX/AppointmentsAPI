using AppointmentsAPI.Application.DTOs.AppointmentResult;
using AppointmentsAPI.Application.DTOs.NotificationService;

namespace AppointmentsAPI.Application.Abstractions.NotificationService;

public interface INotificationService
{
    Task SendAppointmentResultAsync(SendResultEmailDTO dto, CancellationToken cancellationToken = default);
    Task SendReminderNotificationAsync(SendReminderNotificationDTO dto, CancellationToken cancellationToken = default);
}
