using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Domain.Models;

namespace AppointmentsAPI.Application.SyncServices;

public class PatientSyncService : IPatientSyncService
{
    private readonly IRepository<Patient, Guid> _patientRepository;

    public PatientSyncService(IRepository<Patient, Guid> patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task CreatePatientAsync(SyncPatientDTO dto)
    {
        var existing = await _patientRepository.GetByIdAsync(dto.Id);
        if (existing != null) return;

        var patient = new Patient
        {
            Id = dto.Id,
            AccountId = dto.AccountId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            MiddleName = dto.MiddleName,
            PhoneNumber = dto.PhoneNumber,
            DateOfBirth = dto.DateOfBirth
        };

        await _patientRepository.AddAsync(patient);
        await _patientRepository.SaveChangesAsync();
    }

    public async Task UpdatePatientAsync(SyncPatientDTO dto)
    {
        var patient = await _patientRepository.GetByIdAsync(dto.Id);

        if (patient != null)
        {
            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.MiddleName = dto.MiddleName;
            patient.PhoneNumber = dto.PhoneNumber;
            patient.DateOfBirth = dto.DateOfBirth;

            await _patientRepository.SaveChangesAsync();
        }
    }

    public async Task LinkPatientAsync(SyncPatientLinkDTO dto)
    {
        var patient = await _patientRepository.GetByIdAsync(dto.Id);

        if (patient != null)
        {
            patient.AccountId = dto.AccountId;
            await _patientRepository.SaveChangesAsync();
        }
    }

    public async Task DeletePatientAsync(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);

        if (patient != null)
        {
            _patientRepository.Delete(patient); 
            await _patientRepository.SaveChangesAsync();
        }
    }
}