namespace AppointmentsAPI.Application.Messages;

public record SendAppointmentReminderEvent(
    string ToEmail,
    string PatientFullName,
    string ServiceName,
    string DoctorFullName,
    DateTime TimeSlot,
    DateTime Date
);
