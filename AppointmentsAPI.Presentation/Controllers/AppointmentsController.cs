using AppointmentsAPI.Application.Abstractions.Appointments;
using AppointmentsAPI.Application.DTOs.Appointment;
using AppointmentsAPI.Presentation.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentsAPI.Presentation.Controllers;

[Route("api/[controller]")]
public class AppointmentsController : ApiController
{
    private readonly IAppointmentManagementService _appointmentManagementService;

    public AppointmentsController(IAppointmentManagementService appointmentManagementService)
    {
        _appointmentManagementService = appointmentManagementService;
    }

    [HttpPost("me")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> CreateOwnAppointment(
        [FromBody] CreateAppointmentDTO dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(AccountId)) return Forbid();

        var result = await _appointmentManagementService.CreateOwnAppointmentAsync(dto, AccountId, cancellationToken);
        return HandleResult(result, "Appointment has been created");
    }

    [HttpPost("{patientId:guid}")]
    [Authorize(Roles = "Receptionist")]
    public async Task<IActionResult> CreateAppointment(
        Guid patientId,
        [FromBody] CreateAppointmentDTO dto,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentManagementService.CreateAppointmentAsync(dto, patientId, cancellationToken);
        return HandleResult(result, "Appointment has been created");
    }

    [Authorize(Roles = "Receptionist")]
    [HttpPatch("{id:guid}/reschedule")]
    public async Task<IActionResult> RescheduleAppointment(
        Guid id,
        [FromBody] RescheduleAppointmentDTO dto,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentManagementService.RescheduleAppointmentAsync(id, dto, cancellationToken);
        return HandleResult(result, "Appointment has been rescheduled");
    }

    [Authorize(Roles = "Receptionist")]
    [HttpPatch("{id:guid}/approve")]
    public async Task<IActionResult> ApproveAppointment(Guid id, CancellationToken cancellationToken)
    {
        var result = await _appointmentManagementService.ApproveAppointmentAsync(id, cancellationToken);
        return HandleResult(result, "Appointment has been approved successfully");
    }

    [Authorize(Roles = "Receptionist")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAppointment(Guid id, CancellationToken cancellationToken)
    {
        var result = await _appointmentManagementService.DeleteAppointmentAsync(id, cancellationToken);
        return HandleResult(result, "Appointment has been deleted successfully");
    }

    [Authorize(Roles = "Receptionist")]
    [HttpGet("receptionist")]
    public async Task<IActionResult> GetAppointmentsForReceptionist([FromQuery] GetAppointmentsFilterDTO filter, CancellationToken cancellationToken)
    {
        var result = await _appointmentManagementService.GetFilteredAppointmentsAsync(filter, cancellationToken);
        return HandleResult(result);
    }
}
