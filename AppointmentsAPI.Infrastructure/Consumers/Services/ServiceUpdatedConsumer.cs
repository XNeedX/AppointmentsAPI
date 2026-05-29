using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Domain.Enums;
using InnoClinic.Contracts.Events.Services;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Services;

public class ServiceUpdatedConsumer : IConsumer<IServiceUpdatedEvent>
{
    private readonly IServiceSyncService _serviceSyncService;

    public ServiceUpdatedConsumer(IServiceSyncService serviceSyncService)
    {
        _serviceSyncService = serviceSyncService;
    }

    public Task Consume(ConsumeContext<IServiceUpdatedEvent> context)
    {
        var msg = context.Message;
        var dto = new SyncServiceDTO(msg.Id, msg.Name, (ServiceCategory)msg.Category);
        return _serviceSyncService.UpdateServiceAsync(dto);
    }
}
