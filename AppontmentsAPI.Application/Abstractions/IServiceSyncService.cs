using AppointmentsAPI.Application.DTOs;

namespace AppointmentsAPI.Application.Abstractions;

public interface IServiceSyncService
{
    Task CreateServiceAsync(SyncServiceDTO dto);
    Task UpdateServiceAsync(SyncServiceDTO dto);
    Task UpdateStatusServiceAsync(SyncStatusUpdateDTO dto);
}
