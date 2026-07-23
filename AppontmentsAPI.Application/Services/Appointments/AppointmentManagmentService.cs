using AppointmentsAPI.Application.Abstractions.Appointments;
using AppointmentsAPI.Application.Abstractions.Repositories;
using AppointmentsAPI.Application.DTOs.Appointment;
using AppointmentsAPI.Application.Results;
using AppointmentsAPI.Domain.Enums;
using AppointmentsAPI.Domain.Models;

namespace AppointmentsAPI.Application.Services.Appointments;

public class AppointmentManagementService : IAppointmentManagementService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IRepository<Doctor, Guid> _doctorRepository;
    private readonly IRepository<Office, Guid> _officeRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Service, Guid> _serviceRepository;

    public AppointmentManagementService(
        IAppointmentRepository appointmentRepository,
        IRepository<Doctor, Guid> doctorRepository,
        IRepository<Office, Guid> officeRepository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Service, Guid> serviceRepository)
    {
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
        _officeRepository = officeRepository;
        _patientRepository = patientRepository;
        _serviceRepository = serviceRepository;
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

        var existingAppointments = await _appointmentRepository.FindByFilterAsync(
            a => a.DoctorId == dto.DoctorId && a.TimeSlot == cleanTimeSlot,
            null,
            cancellationToken);

        if (existingAppointments.Any())
            return AppointmentErrors.InvalidTimeSlot;

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

        _appointmentRepository.Update(appointment);

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
}