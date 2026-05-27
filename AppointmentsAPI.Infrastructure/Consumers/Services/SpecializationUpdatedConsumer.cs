using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Domain.Enums;
using InnoClinic.Contracts.Events.Services;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Services;

public sealed class SpecializationUpdatedConsumer : IConsumer<ISpecializationUpdatedEvent>
{
    private readonly ISpecializationSyncService _syncService;

    public SpecializationUpdatedConsumer(ISpecializationSyncService syncService)
    {
        _syncService = syncService;
    }

    public async Task Consume(ConsumeContext<ISpecializationUpdatedEvent> context)
    {
        var message = context.Message;

        var dto = new SyncSpecializationDTO(
            message.Id,
            message.Name,
            (Status)message.Status
        );

        await _syncService.UpdateSpecializationAsync(dto);
    }
}