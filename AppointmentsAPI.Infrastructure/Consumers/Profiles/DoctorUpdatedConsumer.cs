using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using InnoClinic.Contracts.Events.Profiles;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Profiles;

public class DoctorUpdatedConsumer : IConsumer<IDoctorUpdatedEvent>
{
    private readonly IDoctorSyncService _doctorSyncService;

    public DoctorUpdatedConsumer(IDoctorSyncService doctorSyncService)
    {
        _doctorSyncService = doctorSyncService;
    }

    public async Task Consume(ConsumeContext<IDoctorUpdatedEvent> context)
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

        await _doctorSyncService.UpdateDoctorAsync(dto);
    }
}
