using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Domain.Models;

namespace AppointmentsAPI.Application.SyncServices;

public class OfficeSyncService : IOfficeSyncService
{
    private readonly IRepository<Office, Guid> _officeRepository;

    public OfficeSyncService(IRepository<Office, Guid> officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task CreateOfficeAsync(SyncOfficeDTO dto)
    {
        var existing = await _officeRepository.GetByIdAsync(dto.Id);
        if (existing != null) return;

        var office = new Office
        {
            Id = dto.Id,
            Address = dto.Address
        };

        await _officeRepository.AddAsync(office);
        await _officeRepository.SaveChangesAsync();
    }

    public async Task UpdateOfficeAsync(SyncOfficeDTO dto)
    {
        var office = await _officeRepository.GetByIdAsync(dto.Id);

        if(office != null)
        {
            office.Address = dto.Address;

            await _officeRepository.SaveChangesAsync();
        }
    }

    public async Task UpdateOfficeStatusAsync(SyncOfficeStatusUpdateDTO dto)
    {
        var office = await _officeRepository.GetByIdAsync(dto.Id);

        if(office != null)
        {
            office.Status = dto.Status;

            await _officeRepository.SaveChangesAsync();
        }
    }
}
