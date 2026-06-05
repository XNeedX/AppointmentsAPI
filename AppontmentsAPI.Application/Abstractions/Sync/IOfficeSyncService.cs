using AppointmentsAPI.Application.DTOs.Sync;

namespace AppointmentsAPI.Application.Abstractions.Sync;

public interface IOfficeSyncService
{
    Task CreateOfficeAsync(SyncOfficeDTO dto);
    Task UpdateOfficeAsync(SyncOfficeDTO dto);
    Task UpdateOfficeStatusAsync(SyncOfficeStatusUpdateDTO dto);
}
