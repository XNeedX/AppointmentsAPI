using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using InnoClinic.Contracts.Events.Profiles;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Profiles;

public class LinkPatientConsumer : IConsumer<IPatientProfileLinkedEvent>
{
    private readonly IPatientSyncService _patientSyncService;

    public LinkPatientConsumer(IPatientSyncService patientSyncService)
    {
        _patientSyncService = patientSyncService;
    }

    public Task Consume(ConsumeContext<IPatientProfileLinkedEvent> context)
    {
        var msg = context.Message;

        var dto = new SyncPatientLinkDTO
        (
            msg.Id,
            msg.AccountId
        );

        return _patientSyncService.LinkPatientAsync(dto);
    }
}
