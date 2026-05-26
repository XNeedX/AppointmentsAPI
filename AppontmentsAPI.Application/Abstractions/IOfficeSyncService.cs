using AppointmentsAPI.Application.DTOs;

namespace AppointmentsAPI.Application.Abstractions;

public interface IOfficeSyncService
{
    Task CreateOfficeAsync(SyncOfficeDTO dto);
    Task UpdateOfficeAsync(SyncOfficeDTO dto);
    Task UpdateOfficeStatusAsync(SyncOfficeStatusUpdateDTO dto);
}
