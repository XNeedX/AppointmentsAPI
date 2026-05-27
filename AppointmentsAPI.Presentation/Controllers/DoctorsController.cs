using AppointmentsAPI.Application.Abstractions;
using AppointmentsAPI.Domain.Enums;
using AppointmentsAPI.Domain.Models;
using AppointmentsAPI.Presentation.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentsAPI.Presentation.Controllers;

[Route("api/[controller]")]
public class DoctorsController : ApiController
{
    private readonly IRepository<Doctor, Guid> _doctorRepository;

    public DoctorsController(IRepository<Doctor, Guid> doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetActiveDoctors(CancellationToken cancellationToken)
    {
        var doctors = await _doctorRepository.FindByFilterAsync(
            d => d.Status == Status.Active,
            cancellationToken);

        return Ok(ApiResponse<IEnumerable<Doctor>>.Success(doctors));
    }
}