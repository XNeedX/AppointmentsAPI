using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Domain.Enums;
using AppointmentsAPI.Domain.Models;
using AppointmentsAPI.Presentation.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentsAPI.Presentation.Controllers;

[Route("api/[controller]")]
public class SpecializationsController : ApiController
{
    private readonly IRepository<Specialization, Guid> _specializationRepository;

    public SpecializationsController(IRepository<Specialization, Guid> specializationRepository)
    {
        _specializationRepository = specializationRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetActiveSpecializations(CancellationToken cancellationToken)
    {
        var specializations = await _specializationRepository.FindByFilterAsync(
            s => s.Status == Status.Active,
            cancellationToken);

        return Ok(ApiResponse<IEnumerable<Specialization>>.Success(specializations));
    }
}