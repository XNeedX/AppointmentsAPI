using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentsAPI.Presentation.Controllers;

[Route("api/[controller]")]
public class AppointmentsController : ApiController
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpPost("{patientId:guid}")]
    public async Task<IActionResult> CreateAppointment(
        Guid patientId, 
        [FromBody] CreateAppointmentDTO dto, 
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.CreateAppointmentAsync(dto, patientId, cancellationToken);

        return HandleResult(result, "Appointment has been created");
    }

    // [Authorize(Roles = "Doctor")] 
    [HttpPost("{appointmentId:guid}/results")]
    public async Task<IActionResult> CreateResult(
        Guid appointmentId,
        [FromBody] CreateAppointmentResultDTO dto, 
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.CreateAppointmentResultAsync(appointmentId, dto, cancellationToken);

        return HandleResult(result, "Result has been created successfully");
    }
}
