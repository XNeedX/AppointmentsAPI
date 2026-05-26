using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using InnoClinic.Contracts.Events.Profiles;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Profiles;

public class ReceptionistUpdatedEvent : IConsumer<IReceptionistUpdatedEvent>
{
    private readonly IReceptionistSyncService _receptionistSyncService;

    public ReceptionistUpdatedEvent(IReceptionistSyncService receptionistSyncService)
    {
        _receptionistSyncService = receptionistSyncService;
    }

    public async Task Consume(ConsumeContext<IReceptionistUpdatedEvent> context)
    {
        var msg = context.Message;

        var dto = new SyncReceptionistDTO
        (
            msg.Id,
            msg.FirstName,
            msg.LastName,
            msg.MiddleName
        );

        await _receptionistSyncService.UpdateReceptionistAsync(dto);
    }
}
