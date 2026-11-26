using AppointmentsAPI.Application.Abstractions.Appointments;
using AppointmentsAPI.Application.Abstractions.Repositories;
using AppointmentsAPI.Application.Abstractions.Schedules;
using AppointmentsAPI.Application.Configurations;
using AppointmentsAPI.Application.DTOs.Appointment;
using AppointmentsAPI.Application.Results;
using AppointmentsAPI.Domain.Enums;
using AppointmentsAPI.Domain.Models;
using AppointmentsAPI.Domain.Extensions;
using Microsoft.Extensions.Options;

namespace AppointmentsAPI.Application.Services.Appointments;

public class AppointmentManagementService : IAppointmentManagementService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IRepository<Doctor, Guid> _doctorRepository;
    private readonly IRepository<Office, Guid> _officeRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Service, Guid> _serviceRepository;

    private readonly IScheduleService _scheduleService;
    private readonly TimeZoneInfo _clinicTimeZone;

    public AppointmentManagementService(
        IAppointmentRepository appointmentRepository,
        IRepository<Doctor, Guid> doctorRepository,
        IRepository<Office, Guid> officeRepository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Service, Guid> serviceRepository,
        IScheduleService scheduleService,
        IOptions<InnoClinicOptions> options)
    {
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
        _officeRepository = officeRepository;
        _patientRepository = patientRepository;
        _serviceRepository = serviceRepository;
        _scheduleService = scheduleService;
        _clinicTimeZone = TimeZoneInfo.FindSystemTimeZoneById(options.Value.TimeZoneId);
    }

    public async Task<Result> CreateOwnAppointmentAsync(
        CreateAppointmentDTO dto, string accountId, CancellationToken cancellationToken = default)
    {
        var patients = await _patientRepository.FindByFilterAsync(
            patient => patient.AccountId == accountId, cancellationToken: cancellationToken);
        var matches = patients.Take(2).ToArray();
        if (matches.Length != 1) return AppointmentErrors.PatientNotFound;

        return await CreateAppointmentAsync(dto, matches[0].Id, cancellationToken);
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

        return await _appointmentRepository.SaveSchedulingChangeAsync(async () =>
        {
            var validation = await ValidateSlotAsync(dto.DoctorId, dto.ServiceId, dto.Date, dto.TimeSlot, null, cancellationToken);
            if (validation.IsFailure) return validation;

            await _appointmentRepository.AddAsync(new Appointment
            {
                Id = Guid.NewGuid(),
                DoctorId = dto.DoctorId,
                ServiceId = dto.ServiceId,
                OfficeId = dto.OfficeId,
                PatientId = patientId,
                Date = DateTime.SpecifyKind(dto.Date.Date, DateTimeKind.Utc),
                TimeSlot = dto.TimeSlot
            }, cancellationToken);

            return Result.Success();
        }, AppointmentErrors.SlotUnavailable, cancellationToken);
    }

    public Task<Result> RescheduleAppointmentAsync(
        Guid appointmentId, RescheduleAppointmentDTO dto, CancellationToken cancellationToken = default)
    {
        return _appointmentRepository.SaveSchedulingChangeAsync(async () =>
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return AppointmentErrors.NotFound;
            if (appointment.IsApproved) return AppointmentErrors.AlreadyApproved;

            var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId, cancellationToken);
            if (doctor == null) return AppointmentErrors.DoctorNotFound;
            if (doctor.Status != Status.Active) return AppointmentErrors.DoctorNotActive;

            var originalDoctor = await _doctorRepository.GetByIdAsync(appointment.DoctorId, cancellationToken);
            if (originalDoctor != null && !string.Equals(
                originalDoctor.Specialization, doctor.Specialization, StringComparison.OrdinalIgnoreCase))
                return AppointmentErrors.DoctorSpecializationMismatch;

            var validation = await ValidateSlotAsync(
                dto.DoctorId, appointment.ServiceId, dto.Date, dto.TimeSlot, appointment.Id, cancellationToken);
            if (validation.IsFailure) return validation;

            appointment.DoctorId = dto.DoctorId;
            appointment.Date = DateTime.SpecifyKind(dto.Date.Date, DateTimeKind.Utc);
            appointment.TimeSlot = dto.TimeSlot;
            _appointmentRepository.Update(appointment);
            return Result.Success();
        }, AppointmentErrors.SlotUnavailable, cancellationToken);
    }

    private async Task<Result> ValidateSlotAsync(
        Guid doctorId, Guid serviceId, DateTime date, DateTime timeSlot, Guid? excludeId, CancellationToken cancellationToken)
    {
        if (date.Year is < 2 or > 9998 || timeSlot.Kind != DateTimeKind.Utc)
            return AppointmentErrors.InvalidTimeSlot;

        var localTime = TimeZoneInfo.ConvertTimeFromUtc(timeSlot, _clinicTimeZone);
        if (date.Date != localTime.Date || localTime.Ticks % TimeSpan.FromMinutes(10).Ticks != 0)
            return AppointmentErrors.InvalidTimeSlot;

        var slots = await _scheduleService.GetAvailableTimeSlotsAsync(doctorId, serviceId, date, cancellationToken, excludeId);
        return slots.Contains(localTime.TimeOfDay) ? Result.Success() : AppointmentErrors.SlotUnavailable;
    }

    public Task<Result> ApproveAppointmentAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        return _appointmentRepository.SaveSchedulingChangeAsync(async () =>
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return AppointmentErrors.NotFound;
            if (appointment.IsApproved) return AppointmentErrors.AlreadyApproved;

            appointment.IsApproved = true;
            _appointmentRepository.Update(appointment);
            return Result.Success();
        }, AppointmentErrors.AppointmentChanged, cancellationToken);
    }

    public async Task<Result> DeleteAppointmentAsync(
           Guid appointmentId,
           CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);

        if (appointment == null)
            return AppointmentErrors.NotFound;

        _appointmentRepository.Delete(appointment, cancellationToken);
        await _appointmentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<IEnumerable<ViewAppointmentListDTO>>> GetFilteredAppointmentsAsync(GetAppointmentsFilterDTO filter, CancellationToken cancellationToken = default)
    {
        var date = filter.Date.HasValue
            ? DateTime.SpecifyKind(filter.Date.Value.Date, DateTimeKind.Utc)
            : (DateTime?)null;
        var appointments = await _appointmentRepository.FindByFilterAsync(
            a =>
                (!date.HasValue || a.Date.Date == date.Value) &&

                (string.IsNullOrEmpty(filter.DoctorName) ||
                    a.Doctor.FirstName.Contains(filter.DoctorName) ||
                    a.Doctor.LastName.Contains(filter.DoctorName)) &&

                (string.IsNullOrEmpty(filter.ServiceName) || a.Service.Name.Contains(filter.ServiceName)) &&

                (!filter.IsApproved.HasValue || a.IsApproved == filter.IsApproved.Value) &&

                (!filter.OfficeId.HasValue || a.OfficeId == filter.OfficeId.Value),

            query => query.OrderBy(a => a.TimeSlot)
                          .ThenBy(a => a.Doctor.LastName)
                          .ThenBy(a => a.Doctor.FirstName)
                          .ThenBy(a => a.Service.Name),

            cancellationToken,
            a => a.Doctor,
            a => a.Patient,
            a => a.Service
        );

        var responseList = appointments.Select(a =>
        {
            int durationMinutes = a.Service.Category.GetDurationMinutes();

            return new ViewAppointmentListDTO(
                a.Id,
                a.TimeSlot,
                a.TimeSlot.AddMinutes(durationMinutes),
                $"{a.Doctor.LastName} {a.Doctor.FirstName} {a.Doctor.MiddleName}".Trim(),
                $"{a.Patient.LastName} {a.Patient.FirstName} {a.Patient.MiddleName}".Trim(),
                a.Patient.PhoneNumber,
                a.Service.Name,
                a.IsApproved,
                a.PatientId,
                a.DoctorId,
                a.ServiceId,
                a.OfficeId,
                a.Date
            );
        });

        return Result<IEnumerable<ViewAppointmentListDTO>>.Success(responseList);
    }
}
