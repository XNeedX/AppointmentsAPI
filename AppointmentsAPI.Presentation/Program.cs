using AppointmentsApi.Extensions;
using AppointmentsAPI.Application.Extensions;
using AppointmentsAPI.Infrastructure.Extensions;
using AppointmentsAPI.Infrastructure.Data;
using AppointmentsAPI.Presentation.Filters;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddApplicationLayer(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient("Profiles", client =>
{
    var url = builder.Configuration["ProfilesApiBaseUrl"] ?? "http://localhost:5154";
    client.BaseAddress = new Uri(url.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(3);
});

builder.Services.AddKeycloakAuth(builder.Configuration);
builder.Services.AddSwaggerWithAuth(builder.Configuration);

var app = builder.Build();

if (app.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<AppointmentsDbContext>().Database.MigrateAsync();
}

app.UseMiddlewarePipeline();

app.Run();
