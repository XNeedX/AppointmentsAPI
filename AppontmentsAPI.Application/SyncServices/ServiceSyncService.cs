using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Domain.Models;

namespace AppointmentsAPI.Application.SyncServices;

public class ServiceSyncService : IServiceSyncService
{
    private readonly IRepository<Service, Guid> _serviceRepository;

    public ServiceSyncService(IRepository<Service, Guid> serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task CreateServiceAsync(SyncServiceDTO dto)
    {
        var service = new Service
        {
            Id = dto.Id,
            Name = dto.Name
        };

        await _serviceRepository.AddAsync(service);
        await _serviceRepository.SaveChangesAsync();
    }

    public async Task UpdateServiceAsync(SyncServiceDTO dto)
    {
        var service = await _serviceRepository.GetByIdAsync(dto.Id);

        if (service != null)
        {
            service.Name = dto.Name;

            await _serviceRepository.SaveChangesAsync();
        }
    }

    public async Task UpdateStatusServiceAsync(SyncStatusUpdateDTO dto)
    {
        var service = await _serviceRepository.GetByIdAsync(dto.Id);

        if (service != null)
        {
            service.Status = dto.Status;

            await _serviceRepository.SaveChangesAsync();
        }
    }
}
