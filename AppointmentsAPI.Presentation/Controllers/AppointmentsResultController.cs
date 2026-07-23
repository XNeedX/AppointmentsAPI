using AppointmentsAPI.Application.Abstractions.AppointmentResults;
using AppointmentsAPI.Application.DTOs.AppointmentResult;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentsAPI.Presentation.Controllers;

[Route("api/appointments")]
public class AppointmentsResultController : ApiController
{
    private readonly IAppointmentResultService _resultService;

    public AppointmentsResultController(IAppointmentResultService resultService)
    {
        _resultService = resultService;
    }

    // [Authorize(Roles = "Doctor")] 
    [HttpPost("{appointmentId:guid}/results")]
    public async Task<IActionResult> CreateResult(
        Guid appointmentId,
        [FromBody] CreateAppointmentResultDTO dto,
        CancellationToken cancellationToken)
    {
        var result = await _resultService.CreateAppointmentResultAsync(appointmentId, dto, cancellationToken);
        return HandleResult(result, "Result has been created successfully");
    }

    // [Authorize(Roles = "Doctor")] 
    [HttpGet("{appointmentId:guid}/results")]
    public async Task<IActionResult> GetAppointmentResult(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        var result = await _resultService.ViewAppointmentResultAsync(appointmentId, cancellationToken);
        return HandleResult(result);
    }

    // [Authorize(Roles = "Doctor")] 
    [HttpPut("results/{resultId:guid}")]
    public async Task<IActionResult> UpdateResult(
        Guid resultId,
        [FromBody] UpdateAppointmentResultDTO dto,
        CancellationToken cancellationToken)
    {
        var result = await _resultService.UpdateAppointmentResultAsync(resultId, dto, cancellationToken);
        return HandleResult(result, "Result has been updated successfully");
    }
}