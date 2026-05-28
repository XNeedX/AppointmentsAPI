using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Application.Results;
using AppointmentsAPI.Domain.Enums;
using AppointmentsAPI.Domain.Models;

namespace AppointmentsAPI.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IRepository<Doctor, Guid> _doctorRepository;
    private readonly IRepository<Office, Guid> _officeRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Service, Guid> _serviceRepository;
    private readonly IRepository<AppointmentResult, Guid> _appointmentResultRepository;

    public AppointmentService(IAppointmentRepository appointmentRepository,
        IRepository<Doctor, Guid> doctorRepository,
        IRepository<Office, Guid> officeRepository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Service, Guid> serviceRepository)
    {
        _doctorRepository = doctorRepository;
        _officeRepository = officeRepository;
        _patientRepository = patientRepository;
        _serviceRepository = serviceRepository;
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

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            DoctorId = dto.DoctorId,
            ServiceId = dto.ServiceId,
            OfficeId = dto.OfficeId,
            PatientId = patientId,
            Date = dto.Date,
            TimeSlot = dto.TimeSlot
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
}
