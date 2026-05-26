using AppointmentsAPI.Application.DTOs;

namespace AppointmentsAPI.Application.Abstractions;

public interface IPatientSyncService
{
    Task CreatePatientAsync(SyncPatientDTO dto);
    Task UpdatePatientAsync(SyncPatientDTO dto);
    Task LinkPatientAsync(SyncPatientLinkDTO dto);
    Task DeletePatientAsync(Guid id);
}
