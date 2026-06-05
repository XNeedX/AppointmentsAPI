using AppointmentsAPI.Application.Abstractions.Sync;
using InnoClinic.Contracts.Events.Profiles;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Profiles;

public class ReceptionistDeletedConsumer : IConsumer<IReceptionistDeletedEvent>
{
    private readonly IReceptionistSyncService _receptionistSyncService;

    public ReceptionistDeletedConsumer(IReceptionistSyncService receptionistSyncService)
    {
        _receptionistSyncService = receptionistSyncService;
    }

    public async Task Consume(ConsumeContext<IReceptionistDeletedEvent> context)
    {
        var msg = context.Message;

        await _receptionistSyncService.DeleteReceptionistAsync(msg.Id);
    }
}
