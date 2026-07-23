using AppointmentsApi.Extensions;
using AppointmentsAPI.Application.Extensions;
using AppointmentsAPI.Infrastructure.Extensions;
using AppointmentsAPI.Presentation.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddApplicationLayer(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddKeycloakAuth(builder.Configuration);
builder.Services.AddSwaggerWithAuth(builder.Configuration);

var app = builder.Build();

app.UseMiddlewarePipeline();

app.Run();
