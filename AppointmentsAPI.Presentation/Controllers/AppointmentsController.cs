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

    // [Authorize(Roles = "Receptionist")] 
    [HttpPatch("{id:guid}/approve")]
    public async Task<IActionResult> ApproveAppointment(Guid id, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.ApproveAppointmentAsync(id, cancellationToken);
        return HandleResult(result, "Appointment has been approved successfully");
    }

    // [Authorize(Roles = "Receptionist")] 
    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> DeleteAppointment(Guid id, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.DeleteAppointmentAsync(id, cancellationToken);
        return HandleResult(result, "Appointment has been deleted successfully");
    }
}
