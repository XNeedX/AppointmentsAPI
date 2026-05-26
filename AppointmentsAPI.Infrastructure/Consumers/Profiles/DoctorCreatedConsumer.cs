using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using InnoClinic.Contracts.Events.Profiles;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Profiles;

public class DoctorCreatedConsumer : IConsumer<IDoctorCreatedEvent>
{
    private readonly IDoctorSyncService _doctorSyncService;

    public DoctorCreatedConsumer(IDoctorSyncService doctorSyncService)
    {
        _doctorSyncService = doctorSyncService;
    }

    public async Task Consume(ConsumeContext<IDoctorCreatedEvent> context)
    {
        var msg = context.Message;

        var dto = new SyncDoctorDTO
        (
            msg.Id,
            msg.FirstName,
            msg.LastName,
            msg.MiddleName,
            msg.Specialization
        );

        await _doctorSyncService.CreateDoctorAsync(dto);
    }
}
