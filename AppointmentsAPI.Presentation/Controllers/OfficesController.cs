using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Domain.Enums;
using AppointmentsAPI.Domain.Models;
using AppointmentsAPI.Presentation.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentsAPI.Presentation.Controllers;

[Route("api/[controller]")]
public class OfficesController : ApiController
{
    private readonly IRepository<Office, Guid> _officeRepository;

    public OfficesController(IRepository<Office, Guid> officeRepository)
    {
        _officeRepository = officeRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetActiveOffices(CancellationToken cancellationToken)
    {
        var offices = await _officeRepository.FindByFilterAsync(
            o => o.Status == Status.Active,
            cancellationToken);

        return Ok(ApiResponse<IEnumerable<Office>>.Success(offices));
    }
}