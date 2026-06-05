using FluentValidation;
using AppointmentsAPI.Application.DTOs.Appointment;

namespace AppointmentsAPI.Application;

public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentDTO>
{
    public CreateAppointmentValidator()
    {
        RuleFor(x => x.ServiceId)
            .NotEmpty().WithMessage("Please, choose the service");
    }
}