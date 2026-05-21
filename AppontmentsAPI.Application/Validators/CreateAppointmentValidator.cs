using FluentValidation;
using AppointmentsAPI.Application.DTOs;

namespace AppointmentsAPI.Application;

public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentDTO>
{
    public CreateAppointmentValidator()
    {
        // Отрабатываем проверку на пустое поле и отдаем нужный текст ошибки
        RuleFor(x => x.ServiceId)
            .NotEmpty().WithMessage("Please, choose the service");
    }
}