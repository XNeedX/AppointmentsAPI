using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Application.Models;
using AppointmentsAPI.Application.Results;
using AppointmentsAPI.Domain.Enums;
using AppointmentsAPI.Domain.Models;
using InnoClinic.Contracts.Events.Appointments;
using MassTransit;

namespace AppointmentsAPI.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IRepository<Doctor, Guid> _doctorRepository;
    private readonly IRepository<Office, Guid> _officeRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Service, Guid> _serviceRepository;
    private readonly IRepository<AppointmentResult, Guid> _appointmentResultRepository;
    private readonly IPDFGeneratorService _pdfGeneratorService;
    private readonly IPublishEndpoint _publishEndpoint;

    public AppointmentService(IAppointmentRepository appointmentRepository,
        IRepository<Doctor, Guid> doctorRepository,
        IRepository<Office, Guid> officeRepository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Service, Guid> serviceRepository,
        IPublishEndpoint publishEndpoint)
    {
        _doctorRepository = doctorRepository;
        _officeRepository = officeRepository;
        _patientRepository = patientRepository;
        _serviceRepository = serviceRepository;
        _publishEndpoint = publishEndpoint;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result> CreateAppointmentAsync(
        CreateAppointmentDTO dto,
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);
        if (patient == null) return AppointmentErrors.PatientNotFound;
        if (patient.Status != Status.Active) return AppointmentErrors.PatientNotActive;

        var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId, cancellationToken);
        if (doctor == null) return AppointmentErrors.DoctorNotFound;
        if (doctor.Status != Status.Active) return AppointmentErrors.DoctorNotActive;

        var office = await _officeRepository.GetByIdAsync(dto.OfficeId, cancellationToken);
        if (office == null) return AppointmentErrors.OfficeNotFound;
        if (office.Status != Status.Active) return AppointmentErrors.OfficeNotActive;

        var service = await _serviceRepository.GetByIdAsync(dto.ServiceId, cancellationToken);
        if (service == null) return AppointmentErrors.ServiceNotFound;
        if (service.Status != Status.Active) return AppointmentErrors.ServiceNotActive;

        var cleanTimeSlot = new DateTime(
        dto.TimeSlot.Year, dto.TimeSlot.Month, dto.TimeSlot.Day,
        dto.TimeSlot.Hour, dto.TimeSlot.Minute, 0, dto.TimeSlot.Kind);

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            DoctorId = dto.DoctorId,
            ServiceId = dto.ServiceId,
            OfficeId = dto.OfficeId,
            PatientId = patientId,
            Date = dto.Date,
            TimeSlot = cleanTimeSlot
        };

        await _appointmentRepository.AddAsync(appointment, cancellationToken);
        await _appointmentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    public async Task<Result> CreateAppointmentResultAsync(
        Guid appointmentId,
        CreateAppointmentResultDTO dto,
        CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);

        if (appointment == null) return AppointmentErrors.NotFound;

        if (appointment.Result != null) return AppointmentErrors.ResultAlreadyExists;

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

        var pdfModel = new AppointmentResultPDF
        {
            PatientFullName = $"{appointment.Patient.LastName} {appointment.Patient.FirstName}",
            DoctorFullName = $"{appointment.Doctor.LastName} {appointment.Doctor.FirstName}",
            Specialization = appointment.Doctor.Specialization,
            Date = appointment.Date,
            Complaints = dto.Complaints,
            Conclusion = dto.Conclusion,
            Recommendations = dto.Recommendations
        };

        var pdfBytes = _pdfGeneratorService.GenerateAppointmentResultPdf(pdfModel);
        var stream = new MemoryStream(pdfBytes);

        await _publishEndpoint.Publish<ISaveAppointmentResultDocumentEvent>(new
        {
            AppointmentId = appointmentId,
            PdfBytes = pdfBytes,
            ContentType = "application/pdf"
        }, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ApproveAppointmentAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);

        if (appointment == null)
            return AppointmentErrors.NotFound;

        if (appointment.IsApproved)
            return AppointmentErrors.AlreadyApproved;

        appointment.IsApproved = true;

        await _appointmentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAppointmentAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);

        if (appointment == null)
            return AppointmentErrors.NotFound;

        _appointmentRepository.Delete(appointment, cancellationToken);

        return Result.Success();
    }

    public async Task<IEnumerable<TimeSpan>> GetAvailableTimeSlotsAsync(
    Guid doctorId,
    Guid serviceId,
    DateTime date,
    CancellationToken cancellationToken = default)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId, cancellationToken);
        if (service == null) return Enumerable.Empty<TimeSpan>();

        int requiredSlots = service.Category switch
        {
            ServiceCategory.Analyses => 1,
            ServiceCategory.Consultations => 2,
            ServiceCategory.Diagnostics => 3,
            _ => 1
        };
        int durationMinutes = requiredSlots * 10;

        var clinicTimeZone = TimeZoneInfo.Local;

        var targetDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Unspecified);

        var localWorkStart = targetDate.AddHours(9);
        var localWorkEnd = targetDate.AddHours(18);

        var utcStartOfDay = TimeZoneInfo.ConvertTimeToUtc(targetDate, clinicTimeZone);
        var utcEndOfDay = utcStartOfDay.AddDays(1);

        var existingAppointments = await _appointmentRepository.FindByFilterAsync(
            a => a.DoctorId == doctorId && a.TimeSlot >= utcStartOfDay && a.TimeSlot < utcEndOfDay,
            cancellationToken,
            a => a.Service);

        var availableSlots = new List<TimeSpan>();

        for (var localSlotTime = localWorkStart; localSlotTime.AddMinutes(durationMinutes) <= localWorkEnd; localSlotTime = localSlotTime.AddMinutes(10))
        {
            bool isFree = true;
            var localSlotEndTime = localSlotTime.AddMinutes(durationMinutes);

            var utcSlotStartTime = TimeZoneInfo.ConvertTimeToUtc(localSlotTime, clinicTimeZone);
            var utcSlotEndTime = TimeZoneInfo.ConvertTimeToUtc(localSlotEndTime, clinicTimeZone);

            foreach (var app in existingAppointments)
            {
                var appStartTime = app.TimeSlot; 

                int appDuration = app.Service.Category switch
                {
                    ServiceCategory.Analyses => 10,
                    ServiceCategory.Consultations => 20,
                    ServiceCategory.Diagnostics => 30,
                    _ => 10
                };
                var appEndTime = appStartTime.AddMinutes(appDuration);

                if (utcSlotStartTime < appEndTime && utcSlotEndTime > appStartTime)
                {
                    isFree = false;
                    break;
                }
            }

            if (isFree)
                availableSlots.Add(localSlotTime.TimeOfDay); 
        }

        return availableSlots;
    }

    public async Task<Result<IEnumerable<DoctorScheduleDTO>>> GetDoctorScheduleAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken = default)
    {
        var targetDate = date.Date;

        var appointments = await _appointmentRepository.FindByFilterAsync(
            a => a.DoctorId == doctorId && a.Date.Date == targetDate,
            cancellationToken,
            a => a.Patient,
            a => a.Service,
            a => a.Result
        );

        var schedule = appointments
            .OrderBy(a => a.TimeSlot)
            .Select(a =>
            {
                int durationMinutes = a.Service.Category switch
                {
                    ServiceCategory.Analyses => 10,
                    ServiceCategory.Consultations => 20,
                    ServiceCategory.Diagnostics => 30,
                    _ => 10
                };

                var patientName = $"{a.Patient.LastName} {a.Patient.FirstName} {a.Patient.MiddleName}".Trim();

                return new DoctorScheduleDTO(
                    a.Id,
                    a.PatientId,
                    patientName,
                    a.Service.Name,
                    a.TimeSlot,
                    a.TimeSlot.AddMinutes(durationMinutes),
                    a.IsApproved,
                    a.Result != null
                );
            });

        return Result<IEnumerable<DoctorScheduleDTO>>.Success(schedule);
    }

    public async Task<Result<IEnumerable<ViewAppointmentListDTO>>> GetFilteredAppointmentsAsync(GetAppointmentsFilterDTO filter, CancellationToken cancellationToken = default)
    {
        var appointments = await _appointmentRepository.FindByFilterAsync(
        a =>
            (!filter.Date.HasValue || a.Date.Date == filter.Date.Value.Date) &&

            (string.IsNullOrEmpty(filter.DoctorName) ||
                a.Doctor.FirstName.Contains(filter.DoctorName) ||
                a.Doctor.LastName.Contains(filter.DoctorName)) &&

            (string.IsNullOrEmpty(filter.ServiceName) || a.Service.Name.Contains(filter.ServiceName)) &&

            (!filter.IsApproved.HasValue || a.IsApproved == filter.IsApproved.Value) &&

            (!filter.OfficeId.HasValue || a.OfficeId == filter.OfficeId.Value),

        cancellationToken,
        a => a.Doctor,
        a => a.Patient,
        a => a.Service
        );

        var sortedAppointments = appointments
        .OrderBy(a => a.TimeSlot)
        .ThenBy(a => a.Doctor.LastName)
        .ThenBy(a => a.Doctor.FirstName)
        .ThenBy(a => a.Service.Name);

        var responseList = sortedAppointments.Select(a =>
        {
            int durationMinutes = a.Service.Category switch
            {
                ServiceCategory.Analyses => 10,
                ServiceCategory.Consultations => 20,
                ServiceCategory.Diagnostics => 30,
                _ => 10
            };

            return new ViewAppointmentListDTO(
                a.Id,
                a.TimeSlot,
                a.TimeSlot.AddMinutes(durationMinutes),
                $"{a.Doctor.LastName} {a.Doctor.FirstName} {a.Doctor.MiddleName}".Trim(),
                $"{a.Patient.LastName} {a.Patient.FirstName} {a.Patient.MiddleName}".Trim(),
                a.Patient.PhoneNumber,
                a.Service.Name,
                a.IsApproved
            );
        });

        return Result<IEnumerable<ViewAppointmentListDTO>>.Success(responseList);
    }

    public async Task<Result<IEnumerable<ViewAppointmentHistoryDTO>>> GetAppointmentHistoryAsync(
    Guid patientId,
    CancellationToken cancellationToken = default)
    {
        var appointments = await _appointmentRepository.FindByFilterAsync(
            a => a.PatientId == patientId,
            cancellationToken,
            a => a.Doctor,
            a => a.Service,
            a => a.Result
        );

        var sortedAppointments = appointments
            .OrderByDescending(a => a.Date.Date)
            .ThenBy(a => a.TimeSlot.TimeOfDay);

        var result = sortedAppointments.Select(a =>
        {
            int durationMinutes = a.Service.Category switch
            {
                ServiceCategory.Analyses => 10,
                ServiceCategory.Consultations => 20,
                ServiceCategory.Diagnostics => 30,
                _ => 10
            };

            var doctorFullName = $"{a.Doctor.LastName} {a.Doctor.FirstName} {a.Doctor.MiddleName}".Trim();

            return new ViewAppointmentHistoryDTO(
                AppointmentId: a.Id,
                Date: a.Date.Date,
                StartTime: a.TimeSlot,
                EndTime: a.TimeSlot.AddMinutes(durationMinutes),
                DoctorFullName: doctorFullName,
                ServiceName: a.Service.Name
            );
        });

        return Result<IEnumerable<ViewAppointmentHistoryDTO>>.Success(result);
    }

    public async Task<Result<IEnumerable<PatientAppointmentHistoryDTO>>> GetPatientAppointmentHistoryAsync(
    Guid patientId,
    CancellationToken cancellationToken = default)
    {
        var appointments = await _appointmentRepository.FindByFilterAsync(
            a => a.PatientId == patientId,
            cancellationToken,
            a => a.Doctor,
            a => a.Service,
            a => a.Result
        );

        var sortedAppointments = appointments
            .OrderByDescending(a => a.Date.Date)
            .ThenBy(a => a.TimeSlot.TimeOfDay);

        var result = sortedAppointments.Select(a =>
        {
            int durationMinutes = a.Service.Category switch
            {
                ServiceCategory.Analyses => 10,
                ServiceCategory.Consultations => 20,
                ServiceCategory.Diagnostics => 30,
                _ => 10
            };

            var doctorFullName = $"{a.Doctor.LastName} {a.Doctor.FirstName} {a.Doctor.MiddleName}".Trim();

            return new PatientAppointmentHistoryDTO(
                AppointmentId: a.Id,
                Date: a.Date.Date,
                StartTime: a.TimeSlot,
                EndTime: a.TimeSlot.AddMinutes(durationMinutes),
                DoctorFullName: doctorFullName,
                ServiceName: a.Service.Name,
                ResultId: a.Result?.Id
            );
        });

        return Result<IEnumerable<PatientAppointmentHistoryDTO>>.Success(result);
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
            PatientDateOfBirth: appointment.Patient.DateOfBirth,
            DoctorFullName: doctorName,
            Specialization: appointment.Doctor.Specialization,
            DoctorId: appointment.DoctorId,
            ServiceName: appointment.Service.Name,
            Complaints: appointment.Result.Complaints,
            Conclusion: appointment.Result.Conclusion,
            Recommendations: appointment.Result.Recommendations
        );

        return Result<ViewAppointmentResultDTO>.Success(dto);
    }

    public async Task<Result<PatientViewAppointmentResultDTO>> GetPatientAppointmentResultAsync(
    Guid appointmentId,
    CancellationToken cancellationToken = default)
    {
        var appointments = await _appointmentRepository.FindByFilterAsync(
            a => a.Id == appointmentId,
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

        var dto = new PatientViewAppointmentResultDTO(
            AppointmentId: appointment.Id,
            Date: appointment.Date,
            PatientFullName: patientName,
            PatientDateOfBirth: appointment.Patient.DateOfBirth,
            DoctorFullName: doctorName,
            Specialization: appointment.Doctor.Specialization,
            ServiceName: appointment.Service.Name,
            Complaints: appointment.Result.Complaints,
            Conclusion: appointment.Result.Conclusion,
            Diagnosis: "", 
            Recommendations: appointment.Result.Recommendations
        );

        return Result<PatientViewAppointmentResultDTO>.Success(dto);
    }
}