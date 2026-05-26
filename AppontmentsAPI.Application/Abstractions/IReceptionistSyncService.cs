using AppointmentsAPI.Application.DTOs;

namespace AppointmentsAPI.Application.Abstractions;

public interface IReceptionistSyncService
{
    Task CreateReceptionistAsync(SyncReceptionistDTO dto);
    Task UpdateReceptionistAsync(SyncReceptionistDTO dto);
    Task DeleteReceptionistAsync(Guid id);
}
