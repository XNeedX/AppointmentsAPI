using AppointmentsAPI.Application.Abstractions.Appointments;
using AppointmentsAPI.Application.DTOs.Appointment;
using AppointmentsAPI.Application.DTOs.AppointmentResult;
using AppointmentsAPI.Presentation.Responses;
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

    // [Authorize(Roles = "Doctor")] 
    [HttpGet("{appointmentId:guid}/results")]
    public async Task<IActionResult> GetAppointmentResult(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.ViewAppointmentResultAsync(appointmentId, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("patient/results/{appointmentId:guid}")]
    public async Task<IActionResult> GetPatientAppointmentResult(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.GetPatientAppointmentResultAsync(appointmentId, cancellationToken);
        return HandleResult(result);
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

    [HttpGet("available-time-slots")]
    public async Task<IActionResult> GetAvailableTimeSlots(
        [FromQuery] Guid doctorId,
        [FromQuery] Guid serviceId,
        [FromQuery] DateTime date,
        CancellationToken cancellationToken)
    {
        var slots = await _appointmentService.GetAvailableTimeSlotsAsync(doctorId, serviceId, date, cancellationToken);
        var formattedSlots = slots.Select(s => s.ToString(@"hh\:mm"));

        return Ok(ApiResponse<IEnumerable<string>>.Success(formattedSlots));
    }

    [HttpGet("available-dates")]
    public async Task<IActionResult> GetAvailableDates(
    [FromQuery] Guid doctorId,
    [FromQuery] Guid serviceId,
    [FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate,
    CancellationToken cancellationToken)
    {
        var availableDates = new List<string>();

        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            var utcDate = DateTime.SpecifyKind(date, DateTimeKind.Utc);

            var slots = await _appointmentService.GetAvailableTimeSlotsAsync(doctorId, serviceId, utcDate, cancellationToken);
            if (slots.Any())
                availableDates.Add(date.ToString("yyyy-MM-dd"));
        }

        return Ok(ApiResponse<IEnumerable<string>>.Success(availableDates));
    }

    // [Authorize(Roles = "Doctor")] 
    [HttpGet("doctor/{doctorId:guid}/schedule")]
    public async Task<IActionResult> GetDoctorSchedule(
        Guid doctorId,
        [FromQuery] DateTime date,
        CancellationToken cancellationToken)
    {
        if (date == default)
            date = DateTime.UtcNow.Date;

        var utcDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

        var result = await _appointmentService.GetDoctorScheduleAsync(doctorId, utcDate, cancellationToken);

        return HandleResult(result);
    }

    // [Authorize(Roles = "Receptionist")] 
    [HttpGet("receptionist")]
    public async Task<IActionResult> GetAppointmentsForReceptionist([FromQuery] GetAppointmentsFilterDTO filter, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.GetFilteredAppointmentsAsync(filter, cancellationToken);

        return HandleResult(result);
    }

    // [Authorize(Roles = "Doctor")]
    [HttpGet("patient/{patientId:guid}/history")]
    public async Task<IActionResult> GetPatientHistory(Guid patientId, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.GetAppointmentHistoryAsync(patientId, cancellationToken);
        return HandleResult(result);
    }

    // [Authorize(Roles = "Doctor")] 
    [HttpPut("results/{resultId:guid}")]
    public async Task<IActionResult> UpdateResult(
        Guid resultId,
        [FromBody] UpdateAppointmentResultDTO dto,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.UpdateAppointmentResultAsync(resultId, dto, cancellationToken);

        return HandleResult(result, "Result has been updated successfully");
    }
}
