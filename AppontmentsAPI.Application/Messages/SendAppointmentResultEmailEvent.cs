namespace AppointmentsAPI.Application.Messages;

public record SendAppointmentResultEmailEvent(
    string ToEmail,
    string PatientName,
    byte[] PdfBytes,
    string FileName
);
