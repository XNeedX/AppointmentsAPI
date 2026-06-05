using AppointmentsAPI.Application.DTOs.Sync;

namespace AppointmentsAPI.Application.Abstractions.Sync;

public interface IReceptionistSyncService
{
    Task CreateReceptionistAsync(SyncReceptionistDTO dto);
    Task UpdateReceptionistAsync(SyncReceptionistDTO dto);
    Task DeleteReceptionistAsync(Guid id);
}
