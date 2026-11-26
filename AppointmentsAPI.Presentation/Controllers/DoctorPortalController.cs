using System.Net;
using System.Net.Http.Json;
using AppointmentsAPI.Application.Abstractions.AppointmentResults;
using AppointmentsAPI.Application.Abstractions.Repositories;
using AppointmentsAPI.Application.Abstractions.Schedules;
using AppointmentsAPI.Application.Configurations;
using AppointmentsAPI.Application.DTOs.Appointment;
using AppointmentsAPI.Application.DTOs.AppointmentResult;
using AppointmentsAPI.Application.DTOs.Schedule;
using AppointmentsAPI.Domain.Models;
using AppointmentsAPI.Presentation.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AppointmentsAPI.Presentation.Controllers;

public sealed record PortalDoctorIdentity(Guid Id);
public sealed record PortalPatientProfile(
    string? PhotoPath, string FirstName, string LastName, string? MiddleName,
    string PhoneNumber, DateTime DateOfBirth);
public sealed record PortalPatient(PortalPatientProfile Profile, IEnumerable<PatientAppointmentHistoryDTO> History);
public sealed record PortalResult(
    Guid AppointmentId, Guid PatientId, DateTime Date, string PatientFullName, DateTime PatientDateOfBirth,
    string DoctorFullName, string Specialization, string ServiceName,
    string? Complaints, string? Conclusion, string? Recommendations, bool HasResult, bool CanEdit);

[Route("api/DoctorPortal")]
[Authorize(Roles = "Doctor")]
public sealed class DoctorPortalController(
    IHttpClientFactory clients,
    IAppointmentRepository appointments,
    IScheduleService schedule,
    IAppointmentResultService results,
    IOptions<InnoClinicOptions> options) : ApiController
{
    [HttpGet("schedule")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DoctorScheduleDTO>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSchedule([FromQuery] DateOnly? date, CancellationToken cancellationToken)
    {
        var doctor = await ReadProfile<PortalDoctorIdentity>("api/Doctors/me", cancellationToken);
        if (doctor.Data is null) return StatusCode(doctor.Status);
        var clinicZone = TimeZoneInfo.FindSystemTimeZoneById(options.Value.TimeZoneId);
        var day = date ?? DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, clinicZone));
        return HandleResult(await schedule.GetDoctorScheduleAsync(
            doctor.Data.Id, day.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), cancellationToken));
    }

    [HttpGet("patients/{patientId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PortalPatient>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPatient(Guid patientId, CancellationToken cancellationToken)
    {
        var doctor = await ReadProfile<PortalDoctorIdentity>("api/Doctors/me", cancellationToken);
        if (doctor.Data is null) return StatusCode(doctor.Status);
        if (!await HasApprovedVisit(doctor.Data.Id, patientId, cancellationToken)) return NotFound();

        var patient = await ReadProfile<PortalPatientProfile>($"api/Patients/{patientId}", cancellationToken);
        if (patient.Data is null) return StatusCode(patient.Status);
        var history = await schedule.GetPatientAppointmentHistoryAsync(patientId, cancellationToken);
        if (history.IsFailure) return HandleFailure(history);

        return Ok(ApiResponse<PortalPatient>.Success(new PortalPatient(patient.Data, history.Value!)));
    }

    [HttpGet("patients/{patientId:guid}/access")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> CheckPatientAccess(Guid patientId, CancellationToken cancellationToken)
    {
        var doctor = await ReadProfile<PortalDoctorIdentity>("api/Doctors/me", cancellationToken);
        if (doctor.Data is null) return StatusCode(doctor.Status);
        return await HasApprovedVisit(doctor.Data.Id, patientId, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpGet("results/{appointmentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PortalResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetResult(Guid appointmentId, CancellationToken cancellationToken)
    {
        var doctor = await ReadProfile<PortalDoctorIdentity>("api/Doctors/me", cancellationToken);
        if (doctor.Data is null) return StatusCode(doctor.Status);
        var visit = await FindVisit(appointmentId, cancellationToken);
        if (visit is null) return NotFound();
        var ownsVisit = visit.DoctorId == doctor.Data.Id;
        if (!ownsVisit && (visit.Result is null || !await HasApprovedVisit(doctor.Data.Id, visit.PatientId, cancellationToken)))
            return NotFound();

        var value = new PortalResult(visit.Id, visit.PatientId, visit.Date,
            PersonName(visit.Patient.LastName, visit.Patient.FirstName, visit.Patient.MiddleName),
            visit.Patient.DateOfBirth,
            PersonName(visit.Doctor.LastName, visit.Doctor.FirstName, visit.Doctor.MiddleName),
            visit.Doctor.Specialization, visit.Service.Name,
            visit.Result?.Complaints, visit.Result?.Conclusion, visit.Result?.Recommendations,
            visit.Result is not null, ownsVisit);
        return Ok(ApiResponse<PortalResult>.Success(value));
    }

    [HttpPost("results/{appointmentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateResult(Guid appointmentId, CreateAppointmentResultDTO body, CancellationToken cancellationToken)
    {
        var doctor = await ReadProfile<PortalDoctorIdentity>("api/Doctors/me", cancellationToken);
        if (doctor.Data is null) return StatusCode(doctor.Status);
        var visit = await FindVisit(appointmentId, cancellationToken);
        if (visit?.DoctorId != doctor.Data.Id) return NotFound();

        return HandleResult(await results.CreateAppointmentResultAsync(appointmentId, body, cancellationToken));
    }

    [HttpPut("results/{appointmentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateResult(Guid appointmentId, UpdateAppointmentResultDTO body, CancellationToken cancellationToken)
    {
        var doctor = await ReadProfile<PortalDoctorIdentity>("api/Doctors/me", cancellationToken);
        if (doctor.Data is null) return StatusCode(doctor.Status);
        var visit = await FindVisit(appointmentId, cancellationToken);
        if (visit?.DoctorId != doctor.Data.Id || visit.Result is null) return NotFound();

        return HandleResult(await results.UpdateAppointmentResultAsync(visit.Result.Id, body, cancellationToken));
    }

    private async Task<Appointment?> FindVisit(Guid id, CancellationToken cancellationToken) =>
        (await appointments.FindByFilterAsync(a => a.Id == id && a.IsApproved,
            null, cancellationToken, a => a.Patient, a => a.Doctor, a => a.Service, a => a.Result)).FirstOrDefault();

    private async Task<bool> HasApprovedVisit(Guid doctorId, Guid patientId, CancellationToken cancellationToken) =>
        (await appointments.FindByFilterAsync(a => a.DoctorId == doctorId && a.PatientId == patientId && a.IsApproved,
            cancellationToken: cancellationToken)).Any();

    private async Task<(int Status, T? Data)> ReadProfile<T>(string path, CancellationToken cancellationToken) where T : class
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.TryAddWithoutValidation("Authorization", Request.Headers.Authorization.ToString());
            using var response = await clients.CreateClient("Profiles").SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode) return ((int)response.StatusCode, null);
            var body = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(cancellationToken);
            return body?.Data is not null ? (200, body.Data) : (502, null);
        }
        catch (HttpRequestException) { return (502, null); }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested) { return (502, null); }
    }

    private static string PersonName(params string?[] parts) =>
        string.Join(" ", parts.Where(part => !string.IsNullOrWhiteSpace(part)));
}
