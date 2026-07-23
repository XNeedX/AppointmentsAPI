using AppointmentsAPI.Application.Abstractions.Repositories;
using AppointmentsAPI.Application.Abstractions.Schedules;
using AppointmentsAPI.Application.Configurations;
using AppointmentsAPI.Application.DTOs.Appointment;
using AppointmentsAPI.Application.DTOs.Schedule;
using AppointmentsAPI.Application.Results;
using AppointmentsAPI.Domain.Extensions;
using AppointmentsAPI.Domain.Models;
using Microsoft.Extensions.Options;

namespace AppointmentsAPI.Application.Services.Schedules;

public class ScheduleService : IScheduleService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IRepository<Service, Guid> _serviceRepository;
    private readonly InnoClinicOptions _innoClinicOptions;

    public ScheduleService(
        IAppointmentRepository appointmentRepository,
        IRepository<Service, Guid> serviceRepository,
        IOptions<InnoClinicOptions> innoClinicOptions)
    {
        _appointmentRepository = appointmentRepository;
        _serviceRepository = serviceRepository;
        _innoClinicOptions = innoClinicOptions.Value;
    }

    public async Task<IEnumerable<TimeSpan>> GetAvailableTimeSlotsAsync(
    Guid doctorId,
    Guid serviceId,
    DateTime date,
    CancellationToken cancellationToken = default)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId, cancellationToken);
        if (service == null) return Enumerable.Empty<TimeSpan>();

        int durationMinutes = service.Category.GetDurationMinutes();

        var timeZoneId = _innoClinicOptions.TimeZoneId;
        var clinicTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        var targetDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Unspecified);

        var localWorkStart = targetDate.AddHours(9);
        var localWorkEnd = targetDate.AddHours(18);

        var utcStartOfDay = TimeZoneInfo.ConvertTimeToUtc(targetDate, clinicTimeZone);
        var utcEndOfDay = utcStartOfDay.AddDays(1);

        var existingAppointments = await _appointmentRepository.FindByFilterAsync(
            a => a.DoctorId == doctorId && a.TimeSlot >= utcStartOfDay && a.TimeSlot < utcEndOfDay,
            null,
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

                int appDuration = app.Service.Category.GetDurationMinutes();

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
            query => query.OrderBy(a => a.TimeSlot), 
            cancellationToken,
            a => a.Patient,
            a => a.Service,
            a => a.Result
        );

        var schedule = appointments
            .OrderBy(a => a.TimeSlot)
            .Select(a =>
            {
                int durationMinutes = a.Service.Category.GetDurationMinutes();

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

    public async Task<Result<IEnumerable<ViewAppointmentHistoryDTO>>> GetAppointmentHistoryAsync(
       Guid patientId,
       CancellationToken cancellationToken = default)
    {
        var appointments = await _appointmentRepository.FindByFilterAsync(
            a => a.PatientId == patientId,
            query => query.OrderByDescending(a => a.TimeSlot), 
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
            int durationMinutes = a.Service.Category.GetDurationMinutes();

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
             query => query.OrderByDescending(a => a.TimeSlot), 
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
            int durationMinutes = a.Service.Category.GetDurationMinutes();

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
}