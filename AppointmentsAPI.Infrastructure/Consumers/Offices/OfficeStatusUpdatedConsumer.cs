using AppointmentsAPI.Application.Abstractions.Sync;
using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Application.DTOs.Sync;
using AppointmentsAPI.Domain.Enums;
using InnoClinic.Contracts.Events.Offices;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Offices;

public class OfficeStatusUpdatedConsumer : IConsumer<IOfficeStatusUpdatedEvent>
{
    private readonly IOfficeSyncService _officeSyncService;

    public OfficeStatusUpdatedConsumer(IOfficeSyncService officeSyncService)
    {
        _officeSyncService = officeSyncService;
    }

    public async Task Consume(ConsumeContext<IOfficeStatusUpdatedEvent> context)
    {
        var message = context.Message;

        var dto = new SyncOfficeStatusUpdateDTO(message.Id, (Status)message.Status);

        await _officeSyncService.UpdateOfficeStatusAsync(dto);
    }
}
