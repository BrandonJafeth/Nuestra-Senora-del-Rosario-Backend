using FluentValidation;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;

namespace Infrastructure.Validations.Administrative
{
    public class ResidentFromApplicantDtoValidator : AbstractValidator<ResidentFromApplicantDto>
    {
        public ResidentFromApplicantDtoValidator()
        {
            RuleFor(x => x.Id_ApplicationForm)
                .GreaterThan(0).WithMessage("El ID del formulario de aplicación es requerido y debe ser válido.");

            RuleFor(x => x.Id_Room)
                .GreaterThan(0).WithMessage("El ID de la habitación es requerido y debe ser válido.");

            RuleFor(x => x.Sexo)
                .NotEmpty().WithMessage("El sexo del residente es requerido.")
                .Must(x => x == "Femenino" || x == "Masculino")
                .WithMessage("El sexo debe ser 'Femenino' o 'Masculino'.");

            RuleFor(x => x.Id_DependencyLevel)
                .GreaterThan(0).WithMessage("El nivel de dependencia es requerido y debe ser válido.");
        }
    }
}
