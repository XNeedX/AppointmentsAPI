using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using InnoClinic.Contracts.Events.Profiles;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Profiles;

public class PatientCreatedConsumer : IConsumer<IPatientCreatedEvent>
{
    private readonly IPatientSyncService _patientSyncService;

    public PatientCreatedConsumer(IPatientSyncService patientSyncService)
    {
        _patientSyncService = patientSyncService;
    }  

    public async Task Consume(ConsumeContext<IPatientCreatedEvent> context)
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

        await _patientSyncService.CreatePatientAsync(dto);
    }
}
