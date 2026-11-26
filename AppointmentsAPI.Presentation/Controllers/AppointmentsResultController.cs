using AppointmentsAPI.Application.Abstractions.AppointmentResults;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("{appointmentId:guid}/my-result")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> GetOwnAppointmentResult(Guid appointmentId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(AccountId)) return Forbid();

        return HandleResult(await _resultService.ViewAppointmentResultAsync(appointmentId, cancellationToken, AccountId));
    }

}
