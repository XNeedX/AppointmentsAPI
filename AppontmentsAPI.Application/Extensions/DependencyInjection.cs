using AppointmentsAPI.Application.Abstractions.AppointmentResults;
using AppointmentsAPI.Application.Abstractions.Appointments;
using AppointmentsAPI.Application.Abstractions.NotificationService;
using AppointmentsAPI.Application.Abstractions.Schedules;
using AppointmentsAPI.Application.Abstractions.Sync;
using AppointmentsAPI.Application.Configurations;
using AppointmentsAPI.Application.Services.AppointmentResults;
using AppointmentsAPI.Application.Services.Appointments;
using AppointmentsAPI.Application.Services.NotificationService;
using AppointmentsAPI.Application.Services.Schedules;
using AppointmentsAPI.Application.Services.SyncServices;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentsAPI.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<InnoClinicOptions>(configuration.GetSection(InnoClinicOptions.SectionName));

        services.AddScoped<IDoctorSyncService, DoctorSyncService>();
        services.AddScoped<IPatientSyncService, PatientSyncService>();
        services.AddScoped<IReceptionistSyncService, ReceptionistSyncService>();
        services.AddScoped<IServiceSyncService, ServiceSyncService>(); 
        services.AddScoped<IOfficeSyncService, OfficeSyncService>();
        services.AddScoped<ISpecializationSyncService, SpecializationSyncService>();

        services.AddScoped<IAppointmentManagementService, AppointmentManagementService>();
        services.AddScoped<IAppointmentResultService, AppointmentResultService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<INotificationService, NotificationService>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
