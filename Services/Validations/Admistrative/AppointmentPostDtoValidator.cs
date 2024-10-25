using FluentValidation;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Validations.Admistrative
{
    public class AppointmentPostDtoValidator : AbstractValidator<AppointmentPostDto>
    {
        public AppointmentPostDtoValidator()
        {
            RuleFor(x => x.Id_Resident)
                .NotEmpty().WithMessage("El ID del residente es obligatorio.");

            RuleFor(x => x.Id_Companion)
                .NotEmpty().WithMessage("El ID del acompañante es obligatorio.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("La fecha de la cita es obligatoria.")
                .GreaterThan(DateTime.Now).WithMessage("La fecha de la cita debe ser en el futuro.");

            RuleFor(x => x.Time)
                .NotEmpty().WithMessage("La hora de la cita es obligatoria.");


        }
    }
}