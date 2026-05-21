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

    public async Task SyncServiceAsync(SyncServiceDTO dto)
    {
        var service = new Service
        {
            Id = dto.Id,
            Name = dto.Name
        };

        await _serviceRepository.AddAsync(service);
        await _serviceRepository.SaveChangesAsync();
    }
}
