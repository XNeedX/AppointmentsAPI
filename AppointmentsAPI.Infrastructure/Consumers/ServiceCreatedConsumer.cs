using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Infrastructure.Data;
using InnoClinic.Contracts.Events.Services;
using MassTransit;

namespace AppointmentsAPI.Application.Consumers;

public sealed class ServiceCreatedConsumer : IConsumer<IServiceCreatedEvent>
{
    private readonly IServiceSyncService _serviceSyncService;

    public ServiceCreatedConsumer(IServiceSyncService serviceSyncService)
    {
        _serviceSyncService = serviceSyncService;
    }

    public async Task Consume(ConsumeContext<IServiceCreatedEvent> context)
    {
        var message = context.Message;

        var dto = new SyncServiceDTO
        (
            message.Id,
            message.Name
        );

        await _serviceSyncService.SyncServiceAsync(dto);
    }
}
