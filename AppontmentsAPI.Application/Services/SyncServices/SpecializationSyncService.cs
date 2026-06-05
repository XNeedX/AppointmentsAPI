using AppointmentsAPI.Application.Abstractions.Repositories;
using AppointmentsAPI.Application.Abstractions.Sync;
using AppointmentsAPI.Application.DTOs.Sync;
using AppointmentsAPI.Domain.Models;

namespace AppointmentsAPI.Application.Services.SyncServices;

public class SpecializationSyncService : ISpecializationSyncService
{
    private readonly IRepository<Specialization, Guid> _repository;

    public SpecializationSyncService(IRepository<Specialization, Guid> repository)
    {
        _repository = repository;
    }

    public async Task CreateSpecializationAsync(SyncSpecializationDTO dto)
    {
        var existing = await _repository.GetByIdAsync(dto.Id);
        if (existing != null) return;

        var specialization = new Specialization
        {
            Id = dto.Id,
            Name = dto.Name,
            Status = dto.Status
        };

        await _repository.AddAsync(specialization);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateSpecializationAsync(SyncSpecializationDTO dto)
    {
        var specialization = await _repository.GetByIdAsync(dto.Id);

        if (specialization != null)
        {
            specialization.Name = dto.Name;
            specialization.Status = dto.Status;

            await _repository.SaveChangesAsync();
        }
    }

    public async Task UpdateSpecializationStatusAsync(SyncSpecializationStatusUpdateDTO dto)
    {
        var specialization = await _repository.GetByIdAsync(dto.Id);

        if (specialization != null)
        {
            specialization.Status = dto.Status;

            await _repository.SaveChangesAsync();
        }
    }
}