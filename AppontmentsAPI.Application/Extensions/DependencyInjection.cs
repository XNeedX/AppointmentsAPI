using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.Services;
using AppointmentsAPI.Application.SyncServices;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentsAPI.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IServiceSyncService, ServiceSyncService>();
        services.AddScoped<IAppointmentService, AppointmentService>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
