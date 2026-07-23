using AppointmentsAPI.Application.DTOs.Appointment;
using FluentValidation;

namespace AppointmentsAPI.Application.Validators;

public class GetAppointmentsFilterValidator : AbstractValidator<GetAppointmentsFilterDTO>
{
    public GetAppointmentsFilterValidator()
    {
        RuleFor(x => x.DoctorName)
            .MaximumLength(100).WithMessage("Doctor name filter cannot exceed 100 characters.");

        RuleFor(x => x.ServiceName)
            .MaximumLength(100).WithMessage("Service name filter cannot exceed 100 characters.");
    }
}