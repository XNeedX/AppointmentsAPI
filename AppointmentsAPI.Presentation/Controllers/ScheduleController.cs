using AppointmentsAPI.Application.Abstractions.Schedules;
using AppointmentsAPI.Presentation.Responses;
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
        CancellationToken cancellationToken)
    {
        var slots = await _scheduleService.GetAvailableTimeSlotsAsync(doctorId, serviceId, date, cancellationToken);
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
            var slots = await _scheduleService.GetAvailableTimeSlotsAsync(doctorId, serviceId, utcDate, cancellationToken);

            if (slots.Any())
                availableDates.Add(date.ToString("yyyy-MM-dd"));
        }

        return Ok(ApiResponse<IEnumerable<string>>.Success(availableDates));
    }

    // [Authorize(Roles = "Doctor")] 
    [HttpGet("doctor/{doctorId:guid}")]
    public async Task<IActionResult> GetDoctorSchedule(
        Guid doctorId,
        [FromQuery] DateTime date,
        CancellationToken cancellationToken)
    {
        if (date == default)
            date = DateTime.UtcNow.Date;

        var utcDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var result = await _scheduleService.GetDoctorScheduleAsync(doctorId, utcDate, cancellationToken);

        return HandleResult(result);
    }

    // [Authorize(Roles = "Doctor")]
    [HttpGet("patient/{patientId:guid}/history")]
    public async Task<IActionResult> GetPatientHistory(Guid patientId, CancellationToken cancellationToken)
    {
        var result = await _scheduleService.GetPatientAppointmentHistoryAsync(patientId, cancellationToken);
        return HandleResult(result);
    }
}