using AppointmentsAPI.Application.Abstractions.Schedules;
using AppointmentsAPI.Presentation.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentsAPI.Presentation.Controllers;

[Route("api/[controller]")]
public class ScheduleController : ApiController
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet("available-time-slots")]
    public async Task<IActionResult> GetAvailableTimeSlots(
        [FromQuery] Guid doctorId,
        [FromQuery] Guid serviceId,
        [FromQuery] DateTime date,
        [FromQuery] Guid? excludeAppointmentId,
        CancellationToken cancellationToken)
    {
        if (excludeAppointmentId.HasValue && !User.IsInRole("Receptionist"))
            return User.Identity?.IsAuthenticated == true ? Forbid() : Challenge();

        if (date.Year is < 2 or > 9998) return BadRequest();
        var slots = await _scheduleService.GetAvailableTimeSlotsAsync(doctorId, serviceId, date, cancellationToken, excludeAppointmentId);
        var formattedSlots = slots.Select(s => s.ToString(@"hh\:mm"));

        return Ok(ApiResponse<IEnumerable<string>>.Success(formattedSlots));
    }

    [HttpGet("available-dates")]
    public async Task<IActionResult> GetAvailableDates(
        [FromQuery] Guid doctorId,
        [FromQuery] Guid serviceId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] Guid? excludeAppointmentId,
        CancellationToken cancellationToken)
    {
        if (excludeAppointmentId.HasValue && !User.IsInRole("Receptionist"))
            return User.Identity?.IsAuthenticated == true ? Forbid() : Challenge();

        if (startDate.Year < 2 || endDate.Year > 9998 || endDate.Date < startDate.Date ||
            (endDate.Date - startDate.Date).TotalDays > 30)
            return BadRequest();

        var availableDates = await _scheduleService.GetAvailableDatesAsync(
            doctorId, serviceId, startDate, endDate, cancellationToken, excludeAppointmentId);
        return Ok(ApiResponse<IEnumerable<string>>.Success(
            availableDates.Select(date => date.ToString("yyyy-MM-dd"))));
    }

    [HttpGet("me/history")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> GetOwnHistory(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(AccountId)) return Forbid();

        return HandleResult(await _scheduleService.GetOwnAppointmentHistoryAsync(AccountId, cancellationToken));
    }

}
