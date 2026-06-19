using AppointmentsAPI.Application.DTOs.AppointmentResult;

namespace AppointmentsAPI.Application.Abstractions.NotificationService;

public interface INotificationService
{
    Task SendAppointmentResultAsync(SendResultEmailDTO dto, CancellationToken cancellationToken = default);
}
