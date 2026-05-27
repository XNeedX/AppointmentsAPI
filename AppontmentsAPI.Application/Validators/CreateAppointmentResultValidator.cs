using AppointmentsAPI.Application.DTOs;
using FluentValidation;

namespace AppointmentsAPI.Application.Validators;

public class CreateAppointmentResultValidator : AbstractValidator<CreateAppointmentResultDTO>
{
    public CreateAppointmentResultValidator()
    {
        RuleFor(x => x.Complaints)
            .NotEmpty().WithMessage("Please, enter the complaints");

        RuleFor(x => x.Conclusion)
            .NotEmpty().WithMessage("Please, enter the conclusion");

        RuleFor(x => x.Recommendations)
            .NotEmpty().WithMessage("Please, enter the recommendations");
    }
}
