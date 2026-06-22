namespace AppointmentsAPI.Application.DTOs.NotificationService;

public record SendReminderNotificationDTO(
    string PatientFullName,
    DateTime Date,
    DateTime Time,
    string ServiceName,
    string DoctorFullName,
    string ToEmail
);
