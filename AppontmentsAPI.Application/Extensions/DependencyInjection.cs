using AppointmentsAPI.Application.Abstractions.Appointments;
using AppointmentsAPI.Application.Abstractions.Sync;
using AppointmentsAPI.Application.Services;
using AppointmentsAPI.Application.Services.SyncServices;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentsAPI.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IServiceSyncService, ServiceSyncService>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
