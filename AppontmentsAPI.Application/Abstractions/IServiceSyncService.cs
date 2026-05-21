using AppointmentsAPI.Application.DTOs;

namespace AppointmentsAPI.Application.Abstractions;

public interface IServiceSyncService
{
    Task SyncServiceAsync(SyncServiceDTO dto);
}
