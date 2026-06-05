using AppointmentsAPI.Application.Abstractions.Sync;
using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Domain.Enums;
using InnoClinic.Contracts.Events.Offices;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Offices;

public class OfficeCreatedConsumer : IConsumer<IOfficeCreatedEvent>
{
    private readonly IOfficeSyncService _officeService;

    public OfficeCreatedConsumer(IOfficeSyncService officeService)
    {
        _officeService = officeService;
    }

    public async Task Consume(ConsumeContext<IOfficeCreatedEvent> context)
    {
        var msg = context.Message;

        var dto = new SyncOfficeDTO
        (
            msg.Id,
            msg.Address,
            (Status)msg.Status
        );

        await _officeService.CreateOfficeAsync(dto);
    }
}
