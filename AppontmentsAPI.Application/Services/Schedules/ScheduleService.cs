using System.Linq.Expressions;
using AppointmentsAPI.Application.Abstractions.Repositories;
using AppointmentsAPI.Application.Abstractions.Schedules;
using AppointmentsAPI.Application.Configurations;
using AppointmentsAPI.Application.DTOs.Appointment;
using AppointmentsAPI.Application.DTOs.Schedule;
using AppointmentsAPI.Application.Results;
using AppointmentsAPI.Domain.Enums;
using AppointmentsAPI.Domain.Extensions;
using AppointmentsAPI.Domain.Models;
using Microsoft.Extensions.Options;

namespace AppointmentsAPI.Application.Services.Schedules;

public class ScheduleService : IScheduleService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IRepository<Service, Guid> _serviceRepository;
    private readonly IRepository<Doctor, Guid> _doctorRepository;
    private readonly TimeZoneInfo _clinicTimeZone;

    public ScheduleService(
        IAppointmentRepository appointmentRepository,
        IRepository<Service, Guid> serviceRepository,
        IRepository<Doctor, Guid> doctorRepository,
        IOptions<InnoClinicOptions> innoClinicOptions)
    {
        _appointmentRepository = appointmentRepository;
        _serviceRepository = serviceRepository;
        _doctorRepository = doctorRepository;
        _clinicTimeZone = TimeZoneInfo.FindSystemTimeZoneById(innoClinicOptions.Value.TimeZoneId);
    }

    public async Task<IEnumerable<TimeSpan>> GetAvailableTimeSlotsAsync(
        Guid doctorId, Guid serviceId, DateTime date, CancellationToken cancellationToken = default,
        Guid? excludeAppointmentId = null)
    {
        var schedule = await LoadAvailabilityAsync(doctorId, serviceId, date, date, cancellationToken, excludeAppointmentId);
        return schedule is null ? [] : FindSlots(date, schedule.Value.Duration, schedule.Value.Bookings).ToList();
    }

    public async Task<IReadOnlyList<DateTime>> GetAvailableDatesAsync(
        Guid doctorId, Guid serviceId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken,
        Guid? excludeAppointmentId)
    {
        var schedule = await LoadAvailabilityAsync(doctorId, serviceId, startDate, endDate, cancellationToken, excludeAppointmentId);
        if (schedule is null) return [];

        var dates = new List<DateTime>();
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            if (FindSlots(date, schedule.Value.Duration, schedule.Value.Bookings).Any())
                dates.Add(date);
        }
        return dates;
    }

    private async Task<(int Duration, IReadOnlyList<(DateTime Start, DateTime End)> Bookings)?> LoadAvailabilityAsync(
        Guid doctorId, Guid serviceId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken,
        Guid? excludeAppointmentId)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);
        if (doctor?.Status != Status.Active) return null;

        var service = await _serviceRepository.GetByIdAsync(serviceId, cancellationToken);
        if (service?.Status != Status.Active) return null;

        var utcStart = ToUtc(startDate.Date).AddMinutes(-30);
        var utcEnd = ToUtc(endDate.Date.AddDays(1));
        var appointments = await _appointmentRepository.FindByFilterAsync(
            a => a.DoctorId == doctorId && a.TimeSlot >= utcStart && a.TimeSlot < utcEnd &&
                (!excludeAppointmentId.HasValue ||
                    a.Id != excludeAppointmentId.Value || a.IsApproved || a.ServiceId != serviceId),
            null, cancellationToken, a => a.Service);

        return (service.Category.GetDurationMinutes(), appointments
            .Select(a => (a.TimeSlot, a.TimeSlot.AddMinutes(a.Service.Category.GetDurationMinutes())))
            .ToList());
    }

    private IEnumerable<TimeSpan> FindSlots(DateTime date, int durationMinutes, IReadOnlyList<(DateTime Start, DateTime End)> bookings)
    {
        var workStart = date.Date.AddHours(9);
        var workEnd = date.Date.AddHours(18);
        for (var slot = workStart; slot.AddMinutes(durationMinutes) <= workEnd; slot = slot.AddMinutes(10))
        {
            var start = ToUtc(slot);
            var end = start.AddMinutes(durationMinutes);
            if (!bookings.Any(booking => start < booking.End && end > booking.Start))
                yield return slot.TimeOfDay;
        }
    }

    private DateTime ToUtc(DateTime localTime) =>
        TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified), _clinicTimeZone);

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

    public Task<Result<IEnumerable<PatientAppointmentHistoryDTO>>> GetPatientAppointmentHistoryAsync(
        Guid patientId, CancellationToken cancellationToken = default) =>
        LoadPatientHistoryAsync(a => a.PatientId == patientId, cancellationToken);

    public Task<Result<IEnumerable<PatientAppointmentHistoryDTO>>> GetOwnAppointmentHistoryAsync(
        string accountId, CancellationToken cancellationToken = default) =>
        LoadPatientHistoryAsync(a => a.Patient.AccountId == accountId, cancellationToken);

    private async Task<Result<IEnumerable<PatientAppointmentHistoryDTO>>> LoadPatientHistoryAsync(
        Expression<Func<Appointment, bool>> filter, CancellationToken cancellationToken)
    {
        var appointments = await _appointmentRepository.FindByFilterAsync(
             filter,
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