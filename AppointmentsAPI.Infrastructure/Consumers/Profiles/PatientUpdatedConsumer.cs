using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using InnoClinic.Contracts.Events.Profiles;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Profiles;

public class PatientUpdatedConsumer : IConsumer<IPatientUpdatedEvent>
{
    private readonly IPatientSyncService _patientSyncService;

    public PatientUpdatedConsumer(IPatientSyncService patientSyncService)
    {
        _patientSyncService = patientSyncService;
    }

    public Task Consume(ConsumeContext<IPatientUpdatedEvent> context)
    {
        var msg = context.Message;

        var dto = new SyncPatientDTO
        (
            msg.Id,
            msg.AccountId,
            msg.FirstName,
            msg.LastName,
            msg.MiddleName,
            msg.PhoneNumber,
            msg.DateOfBirth
        );

        return _patientSyncService.UpdatePatientAsync(dto);
    }
}
