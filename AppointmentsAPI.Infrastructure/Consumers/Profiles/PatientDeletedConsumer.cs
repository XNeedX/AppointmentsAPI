using AppointmentsAPI.Application.Abstractions.Sync;
using InnoClinic.Contracts.Events.Profiles;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Profiles;

public class PatientDeletedConsumer : IConsumer<IPatientProfileDeletedEvent>
{
    private readonly IPatientSyncService _patientSyncService;

    public PatientDeletedConsumer(IPatientSyncService patientSyncService)
    {
        _patientSyncService = patientSyncService;
    }

    public async Task Consume(ConsumeContext<IPatientProfileDeletedEvent> context)
    {
        var msg = context.Message;

        await _patientSyncService.DeletePatientAsync(msg.Id);
    }
}
