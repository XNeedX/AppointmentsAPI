using AppointmentsAPI.Application.DTOs;

namespace AppointmentsAPI.Application.Abstractions;

public interface IDoctorSyncService
{
    Task CreateDoctorAsync(SyncDoctorDTO dto);
    Task UpdateDoctorAsync(SyncDoctorDTO dto);
}
