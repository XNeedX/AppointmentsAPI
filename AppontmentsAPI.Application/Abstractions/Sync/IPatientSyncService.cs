using AppointmentsAPI.Application.DTOs.Sync;

namespace AppointmentsAPI.Application.Abstractions.Sync;

public interface IPatientSyncService
{
    Task CreatePatientAsync(SyncPatientDTO dto);
    Task UpdatePatientAsync(SyncPatientDTO dto);
    Task LinkPatientAsync(SyncPatientLinkDTO dto);
    Task DeletePatientAsync(Guid id);
}
