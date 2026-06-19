namespace AppointmentsAPI.Application.DTOs.AppointmentResult;

public record SendResultEmailDTO(
    string ToEmail,
    string PatientName,
    byte[] PdfBytes,
    string FileName
);

