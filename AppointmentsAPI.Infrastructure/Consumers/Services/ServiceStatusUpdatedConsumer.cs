using AppointmentsAPI.Application.Abstractions.Sync;
using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Domain.Enums;
using InnoClinic.Contracts.Events.Services;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Services;

public class StatusUpdatedConsumer : IConsumer<IServiceStatusUpdatedEvent>
{
    private readonly IServiceSyncService _serviceSyncService;

    public StatusUpdatedConsumer(IServiceSyncService serviceSyncService)
    {
        _serviceSyncService = serviceSyncService;
    }

    public Task Consume(ConsumeContext<IServiceStatusUpdatedEvent> context)
    {
        var msg = context.Message;
        var dto = new SyncStatusUpdateDTO(msg.Id, (Status)msg.Status);
        return _serviceSyncService.UpdateStatusServiceAsync(dto);
    }
}
