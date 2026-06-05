using AppointmentsAPI.Application.Abstractions.Repositories;
using AppointmentsAPI.Application.Abstractions.Sync;
using AppointmentsAPI.Application.DTOs.Sync;
using AppointmentsAPI.Domain.Enums;
using AppointmentsAPI.Domain.Models;

namespace AppointmentsAPI.Application.Services.SyncServices;

public class DoctorSyncService : IDoctorSyncService
{
    private readonly IRepository<Doctor, Guid> _doctorRepository;

    public DoctorSyncService(IRepository<Doctor, Guid> doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task CreateDoctorAsync(SyncDoctorDTO dto)
    {
        var existing = await _doctorRepository.GetByIdAsync(dto.Id);
        if (existing != null) return;

        var doctor = new Doctor
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            MiddleName = dto.MiddleName,
            Specialization = dto.Specialization
        };

        await _doctorRepository.AddAsync(doctor);
        await _doctorRepository.SaveChangesAsync();
    }

    public async Task UpdateDoctorAsync(SyncDoctorDTO dto)
    {
        var doctor = await _doctorRepository.GetByIdAsync(dto.Id);

        if (doctor != null)
        {
            doctor.FirstName = dto.FirstName;
            doctor.LastName = dto.LastName;
            doctor.MiddleName = dto.MiddleName;
            doctor.Specialization = dto.Specialization;

            await _doctorRepository.SaveChangesAsync();
        }
    }
}