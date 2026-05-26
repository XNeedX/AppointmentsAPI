using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using InnoClinic.Contracts.Events.Offices;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Offices;

public class OfficeUpdatedConsumer : IConsumer<IOfficeUpdatedEvent>
{
    private readonly IOfficeSyncService _officeService;

    public OfficeUpdatedConsumer(IOfficeSyncService officeService)
    {
        _officeService = officeService;
    }
    public async Task Consume(ConsumeContext<IOfficeUpdatedEvent> context)
    {
        var msg = context.Message;

        var dto = new SyncOfficeDTO
        (
            msg.Id,
            msg.Address
        );

        await _officeService.UpdateOfficeAsync(dto);
    }
}
