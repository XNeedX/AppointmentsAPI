using AppointmentsAPI.Application.Abstractions.AppointmentResults;
using AppointmentsAPI.Application.Abstractions.Appointments;
using AppointmentsAPI.Application.Abstractions.Repositories;
using AppointmentsAPI.Application.Abstractions.Schedules;
using AppointmentsAPI.Application.Abstractions.Sync;
using AppointmentsAPI.Application.Services.AppointmentResults;
using AppointmentsAPI.Application.Services.Appointments;
using AppointmentsAPI.Application.Services.Schedules;
using AppointmentsAPI.Application.Services.SyncServices;
using AppointmentsAPI.Domain.Models;
using AppointmentsAPI.Infrastructure.Consumers.Services;
using AppointmentsAPI.Infrastructure.Data;
using AppointmentsAPI.Infrastructure.Repositories;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AppointmentsAPI.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppointmentsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumers(Assembly.GetExecutingAssembly());

            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"], "/", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"]);
                    h.Password(configuration["RabbitMQ:Password"]);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IDoctorSyncService, DoctorSyncService>();
        services.AddScoped<IPatientSyncService, PatientSyncService>();
        services.AddScoped<IReceptionistSyncService, ReceptionistSyncService>();
        services.AddScoped<IServiceSyncService, ServiceSyncService>();
        services.AddScoped<IOfficeSyncService, OfficeSyncService>();
        services.AddScoped<ISpecializationSyncService, SpecializationSyncService>();
        services.AddScoped<IPDFGeneratorService, PDFGeneratorService>();

        services.AddScoped<IAppointmentManagementService, AppointmentManagementService>();
        services.AddScoped<IAppointmentResultService, AppointmentResultService>();
        services.AddScoped<IScheduleService, ScheduleService>();

        return services;
    }
}
