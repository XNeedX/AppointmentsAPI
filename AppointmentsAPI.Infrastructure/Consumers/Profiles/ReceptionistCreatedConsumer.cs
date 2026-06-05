using AppointmentsAPI.Application.Abstractions.Sync;
using AppointmentsAPI.Application.DTOs;
using InnoClinic.Contracts.Events.Profiles;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Profiles;

public class ReceptionistCreatedConsumer : IConsumer<IReceptionistCreatedEvent>
{
    private readonly IReceptionistSyncService _receptionistSyncService;

    public ReceptionistCreatedConsumer(IReceptionistSyncService receptionistSyncService)
    {
        _receptionistSyncService = receptionistSyncService;
    }

    public async Task Consume(ConsumeContext<IReceptionistCreatedEvent> context)
    {
        var msg = context.Message;

        var dto = new SyncReceptionistDTO
        (
            msg.Id,
            msg.FirstName,
            msg.LastName,
            msg.MiddleName
        );

        await _receptionistSyncService.CreateReceptionistAsync(dto);
    }
}
