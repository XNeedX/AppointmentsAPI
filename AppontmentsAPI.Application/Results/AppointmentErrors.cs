namespace AppointmentsAPI.Application.Results;

public static class AppointmentErrors
{
    public static readonly Error CreationFailed = new(
        "Appointment.CreationFailed", 
        "Failed to create the appointment.");
    public static readonly Error InvalidTimeSlot = new(
        "Appointment.InvalidTimeSlot",
        "The selected time slot is invalid.",
        ErrorType.Validation);

    public static readonly Error DoctorNotFound = new(
        "Appointment.DoctorNotFound", 
        "Doctor not found.",
        ErrorType.NotFound);
    public static readonly Error DoctorNotActive = new(
        "Appointment.DoctorNotActive", 
        "The selected doctor is not at work (inactive).", 
        ErrorType.Conflict);

    public static readonly Error OfficeNotFound = new(
        "Appointment.OfficeNotFound", 
        "Office not found.",
        ErrorType.NotFound);
    public static readonly Error OfficeNotActive = new(
        "Appointment.OfficeNotActive", 
        "The selected office is not active.", 
        ErrorType.Conflict);

    public static readonly Error ServiceNotFound = new(
        "Appointment.ServiceNotFound", 
        "Service not found.",
        ErrorType.NotFound);
    public static readonly Error ServiceNotActive = new(
        "Appointment.ServiceNotActive", 
        "The selected service is not active.", 
        ErrorType.Conflict);

    public static readonly Error PatientNotFound = new(
        "Appointment.PatientNotFound", 
        "Patient not found.",
        ErrorType.NotFound);
    public static readonly Error PatientNotActive = new(
        "Appointment.PatientNotActive", 
        "Your patient account is not active.", 
        ErrorType.Forbidden);

    public static readonly Error ResultAlreadyExists = new(
        "Appointment.ResultAlreadyExists", 
        "This appointment already has a result.", 
        ErrorType.Conflict);
    public static readonly Error NotFound = new(
        "Appointment.NotFound", 
        "Appointment not found.", 
        ErrorType.NotFound);
    public static readonly Error AlreadyApproved = new(
        "Appointment.AlreadyApproved", 
        "This appointment is already approved.", 
        ErrorType.Conflict);
    public static readonly Error ResultNotFound = new(
        "AppointmentResult.NotFound",
        "Appointment result not found.",
        ErrorType.NotFound);
}