using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Domain.Models;

namespace AppointmentsAPI.Application.SyncServices;

public class ReceptionistSyncService : IReceptionistSyncService
{
    private readonly IRepository<Receptionist, Guid> _receptionistRepository;

    public ReceptionistSyncService(IRepository<Receptionist, Guid> receptionistRepository)
    {
        _receptionistRepository = receptionistRepository;
    }

    public async Task CreateReceptionistAsync(SyncReceptionistDTO dto)
    {
        var existing = await _receptionistRepository.GetByIdAsync(dto.Id);
        if (existing != null) return;

        var receptionist = new Receptionist
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            MiddleName = dto.MiddleName
        };

        await _receptionistRepository.AddAsync(receptionist);
        await _receptionistRepository.SaveChangesAsync();
    }

    public async Task UpdateReceptionistAsync(SyncReceptionistDTO dto)
    {
        var receptionist = await _receptionistRepository.GetByIdAsync(dto.Id);

        if (receptionist != null)
        {
            receptionist.FirstName = dto.FirstName;
            receptionist.LastName = dto.LastName;
            receptionist.MiddleName = dto.MiddleName;

            await _receptionistRepository.SaveChangesAsync();
        }
    }

    public async Task DeleteReceptionistAsync(Guid id)
    {
        var receptionist = await _receptionistRepository.GetByIdAsync(id);

        if (receptionist != null)
        {
            _receptionistRepository.Delete(receptionist);
            await _receptionistRepository.SaveChangesAsync();
        }
    }
}