using AppointmentsAPI.Application.DTOs;

namespace AppointmentsAPI.Application.Abstractions;

public interface ISpecializationSyncService
{
    Task CreateSpecializationAsync(SyncSpecializationDTO dto);
    Task UpdateSpecializationAsync(SyncSpecializationDTO dto);
    Task UpdateSpecializationStatusAsync(SyncSpecializationStatusUpdateDTO dto);
}