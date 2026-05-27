using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Domain.Enums;
using InnoClinic.Contracts.Events.Services;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Services;

public sealed class SpecializationStatusUpdatedConsumer : IConsumer<ISpecializationStatusUpdatedEvent>
{
    private readonly ISpecializationSyncService _syncService;

    public SpecializationStatusUpdatedConsumer(ISpecializationSyncService syncService)
    {
        _syncService = syncService;
    }

    public async Task Consume(ConsumeContext<ISpecializationStatusUpdatedEvent> context)
    {
        var message = context.Message;

        var dto = new SyncSpecializationStatusUpdateDTO(
            message.Id,
            (Status)message.Status
        );

        await _syncService.UpdateSpecializationStatusAsync(dto);
    }
}