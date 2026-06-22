using AppointmentsAPI.Application.Abstractions.AppointmentResults;
using AppointmentsAPI.Application.Abstractions.NotificationService;
using AppointmentsAPI.Application.Abstractions.Repositories;
using AppointmentsAPI.Application.Configurations;
using AppointmentsAPI.Application.DTOs.AppointmentResult;
using AppointmentsAPI.Application.Messages;
using AppointmentsAPI.Application.Models;
using AppointmentsAPI.Application.Results;
using AppointmentsAPI.Domain.Models;
using InnoClinic.Contracts.Events.Appointments;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Options;

namespace AppointmentsAPI.Application.Services.AppointmentResults;

public class AppointmentResultService : IAppointmentResultService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IRepository<AppointmentResult, Guid> _appointmentResultRepository;
    private readonly IPDFGeneratorService _pdfGeneratorService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly InnoClinicOptions _innoClinicOptions;
    private readonly INotificationService _notificationService;
    private readonly IMessageScheduler _messageScheduler;

    public AppointmentResultService(
        IAppointmentRepository appointmentRepository,
        IRepository<AppointmentResult, Guid> resultRepository,
        IPDFGeneratorService pdfGeneratorService,
        IPublishEndpoint publishEndpoint,
        IOptions<InnoClinicOptions> innoClinicOptions,
        INotificationService notificationService,
        IMessageScheduler messageScheduler)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentResultRepository = resultRepository;
        _pdfGeneratorService = pdfGeneratorService;
        _publishEndpoint = publishEndpoint;
        _notificationService = notificationService;
        _innoClinicOptions = innoClinicOptions.Value;
        _messageScheduler = messageScheduler;
    }

    public async Task<Result> CreateAppointmentResultAsync(
            Guid appointmentId,
            CreateAppointmentResultDTO dto,
            CancellationToken cancellationToken = default)
    {
        var appointments = await _appointmentRepository.FindByFilterAsync(
            a => a.Id == appointmentId,
            null,
            cancellationToken,
            a => a.Patient,
            a => a.Doctor,
            a => a.Result
        );

        var appointment = appointments.FirstOrDefault();

        if (appointment == null) return AppointmentErrors.NotFound;

        if (appointment.Result != null) return AppointmentErrors.ResultAlreadyExists;

        if (appointment.IsApproved == false) return AppointmentErrors.NotApproved;

        var result = new AppointmentResult
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointmentId,
            Complaints = dto.Complaints,
            Conclusion = dto.Conclusion,
            Recommendations = dto.Recommendations
        };

        await _appointmentResultRepository.AddAsync(result, cancellationToken);
        await _appointmentResultRepository.SaveChangesAsync(cancellationToken);

        var clinicTimeZone = TimeZoneInfo.FindSystemTimeZoneById(_innoClinicOptions.TimeZoneId);

        var localTimeSlot = TimeZoneInfo.ConvertTimeFromUtc(appointment.TimeSlot, clinicTimeZone);

        var pdfModel = new AppointmentResultPDF
        {
            PatientFullName = $"{appointment.Patient.LastName} {appointment.Patient.FirstName}",
            DoctorFullName = $"{appointment.Doctor.LastName} {appointment.Doctor.FirstName}",
            Specialization = appointment.Doctor.Specialization,
            Date = localTimeSlot,
            Complaints = dto.Complaints,
            Conclusion = dto.Conclusion,
            Recommendations = dto.Recommendations
        };

        var pdfBytes = _pdfGeneratorService.GenerateAppointmentResultPdf(pdfModel);

        await _publishEndpoint.Publish<ISaveAppointmentResultDocumentEvent>(new
        {
            AppointmentId = appointmentId,
            PdfBytes = pdfBytes,
            ContentType = "application/pdf"
        }, cancellationToken);

        if(!string.IsNullOrEmpty(appointment.Patient.Email))
        {
            var fileName = $"AppointmentResult_{appointment.Date:yyyyMMdd}.pdf";
            var patientName = $"{appointment.Patient.FirstName} {appointment.Patient.LastName}";

            await _publishEndpoint.Publish(new SendAppointmentResultEmailEvent(
                appointment.Patient.Email,
                patientName,
                pdfBytes,
                fileName
            ));
        }

        DateTime sendTime = appointment.TimeSlot.AddDays(-1);
        DateTime sendTimeUtc = TimeZoneInfo.ConvertTimeToUtc(sendTime);

        await _messageScheduler.ScheduleSend(
            new Uri("queue:send-appointment-reminder"),
            sendTimeUtc,
            new SendAppointmentReminderEvent(
                appointment.Patient.Email,
                $"{appointment.Patient.FirstName} {appointment.Patient.LastName}",
                appointment.Service.Name,
                $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}", 
                appointment.Date, 
                appointment.TimeSlot
            )
        ); 

        return Result.Success();
    }

    public async Task<Result> UpdateAppointmentResultAsync(
       Guid resultId,
       UpdateAppointmentResultDTO dto,
       CancellationToken cancellationToken = default)
    {
        var existingResult = await _appointmentResultRepository.GetByIdAsync(resultId, cancellationToken);

        if (existingResult == null)
            return AppointmentErrors.ResultNotFound;

        existingResult.Complaints = dto.Complaints;
        existingResult.Conclusion = dto.Conclusion;
        existingResult.Recommendations = dto.Recommendations;

        await _appointmentResultRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<ViewAppointmentResultDTO>> ViewAppointmentResultAsync(
       Guid appointmentId,
       CancellationToken cancellationToken = default)
    {
        var appointments = await _appointmentRepository.FindByFilterAsync(
            a => a.Id == appointmentId,
            null,
            cancellationToken,
            a => a.Patient,
            a => a.Doctor,
            a => a.Service,
            a => a.Result
        );

        var appointment = appointments.FirstOrDefault();

        if (appointment == null)
            return AppointmentErrors.NotFound;

        if (appointment.Result == null)
            return AppointmentErrors.ResultNotFound;

        var patientName = $"{appointment.Patient.LastName} {appointment.Patient.FirstName} {appointment.Patient.MiddleName}".Trim();
        var doctorName = $"{appointment.Doctor.LastName} {appointment.Doctor.FirstName} {appointment.Doctor.MiddleName}".Trim();

        var dto = new ViewAppointmentResultDTO(
            AppointmentId: appointment.Id,
            Date: appointment.Date,
            PatientFullName: patientName,
            DoctorFullName: doctorName,
            PatientDateOfBirth: appointment.Patient.DateOfBirth,
            DoctorId: appointment.DoctorId, 
            Specialization: appointment.Doctor.Specialization,
            ServiceName: appointment.Service.Name,
            Complaints: appointment.Result.Complaints,
            Conclusion: appointment.Result.Conclusion,
            Recommendations: appointment.Result.Recommendations
        );

        return Result<ViewAppointmentResultDTO>.Success(dto);
    }
}