using AppointmentsAPI.Application.DTOs.Sync;

namespace AppointmentsAPI.Application.Abstractions.Sync;

public interface ISpecializationSyncService
{
    Task CreateSpecializationAsync(SyncSpecializationDTO dto);
    Task UpdateSpecializationAsync(SyncSpecializationDTO dto);
    Task UpdateSpecializationStatusAsync(SyncSpecializationStatusUpdateDTO dto);
}