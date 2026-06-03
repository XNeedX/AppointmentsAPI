using AppointmentsAPI.Application.DTOs;
using FluentValidation;

namespace AppointmentsAPI.Application.Validators;

public class UpdateAppointmentResultValidator : AbstractValidator<UpdateAppointmentResultDTO>
{
    public UpdateAppointmentResultValidator()
    {
        RuleFor(x => x.Complaints)
            .NotEmpty().WithMessage("Please, enter the complaints");

        RuleFor(x => x.Conclusion)
            .NotEmpty().WithMessage("Please, enter the conclusion");

        RuleFor(x => x.Recommendations)
            .NotEmpty().WithMessage("Please, enter the recommendations");
    }
}