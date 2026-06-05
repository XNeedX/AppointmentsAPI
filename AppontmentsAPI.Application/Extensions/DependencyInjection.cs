using AppointmentsAPI.Application.Abstractions.Sync;
using AppointmentsAPI.Application.Configurations;
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

        services.AddScoped<IServiceSyncService, ServiceSyncService>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
