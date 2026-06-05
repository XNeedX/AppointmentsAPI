using AppointmentsAPI.Application.DTOs.Sync;

namespace AppointmentsAPI.Application.Abstractions.Sync;

public interface IDoctorSyncService
{
    Task CreateDoctorAsync(SyncDoctorDTO dto);
    Task UpdateDoctorAsync(SyncDoctorDTO dto);
}
