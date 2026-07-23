using AppointmentsAPI.Application.DTOs.Sync;

namespace AppointmentsAPI.Application.Abstractions.Sync;

public interface IServiceSyncService
{
    Task CreateServiceAsync(SyncServiceDTO dto);
    Task UpdateServiceAsync(SyncServiceDTO dto);
    Task UpdateStatusServiceAsync(SyncStatusUpdateDTO dto);
}
