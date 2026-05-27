using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Domain.Enums;
using AppointmentsAPI.Domain.Models;
using AppointmentsAPI.Presentation.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentsAPI.Presentation.Controllers;

[Route("api/[controller]")]
public class ServicesController : ApiController
{
    private readonly IRepository<Service, Guid> _serviceRepository;

    public ServicesController(IRepository<Service, Guid> serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetActiveServices(CancellationToken cancellationToken)
    {
        var services = await _serviceRepository.FindByFilterAsync(
            s => s.Status == Status.Active,
            cancellationToken);

        return Ok(ApiResponse<IEnumerable<Service>>.Success(services));
    }
}