using AppointmentsAPI.Application.Abstractions.Sync;
using AppointmentsAPI.Application.DTOs;
using AppointmentsAPI.Domain.Enums;
using InnoClinic.Contracts.Events.Services;
using MassTransit;

namespace AppointmentsAPI.Infrastructure.Consumers.Services;

public sealed class SpecializationCreatedConsumer : IConsumer<ISpecializationCreatedEvent>
{
    private readonly ISpecializationSyncService _syncService;

    public SpecializationCreatedConsumer(ISpecializationSyncService syncService)
    {
        _syncService = syncService;
    }

    public async Task Consume(ConsumeContext<ISpecializationCreatedEvent> context)
    {
        var message = context.Message;

        var dto = new SyncSpecializationDTO(
            message.Id,
            message.Name,
            (Status)message.Status 
        );

        await _syncService.CreateSpecializationAsync(dto);
    }
}