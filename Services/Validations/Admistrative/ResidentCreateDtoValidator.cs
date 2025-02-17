using FluentValidation;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;

namespace Infrastructure.Validations.Administrative
{
    public class ResidentCreateDtoValidator : AbstractValidator<ResidentCreateDto>
    {
        public ResidentCreateDtoValidator()
        {
            // Validación del nombre del residente
            RuleFor(x => x.Name_RD)
                .NotEmpty().WithMessage("El nombre del residente es requerido.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            // Validación del primer apellido del residente
            RuleFor(x => x.Lastname1_RD)
                .NotEmpty().WithMessage("El primer apellido del residente es requerido.")
                .MaximumLength(100).WithMessage("El apellido no puede exceder los 100 caracteres.");

            // Validación del segundo apellido del residente
            RuleFor(x => x.Lastname2_RD)
                .NotEmpty().WithMessage("El segundo apellido del residente es requerido.")
                .MaximumLength(100).WithMessage("El apellido no puede exceder los 100 caracteres.");

            // Validación de la cédula del residente
            RuleFor(x => x.Cedula_RD)
                .NotEmpty().WithMessage("La cédula del residente es requerida.")
                .Matches(@"^[0-9]{9,12}$").WithMessage("La cédula debe tener entre 9 y 12 dígitos.");

            // Validación del sexo
            RuleFor(x => x.Sexo)
                .NotEmpty().WithMessage("El sexo del residente es requerido.")
                .Must(x => x == "Femenino" || x == "Masculino")
                .WithMessage("El sexo debe ser 'Femenino' o 'Masculino'.");

            // Validación de la fecha de nacimiento
            RuleFor(x => x.FechaNacimiento)
                .LessThan(DateTime.Now).WithMessage("La fecha de nacimiento debe ser anterior a hoy.")
                .NotEmpty().WithMessage("La fecha de nacimiento es requerida.");

            // Validación del ID del guardián
            RuleFor(x => x.Id_Guardian)
                .GreaterThan(0).WithMessage("El ID del guardián es requerido y debe ser válido.");

            // Validación del ID de la habitación
            RuleFor(x => x.Id_Room)
                .GreaterThan(0).WithMessage("El ID de la habitación es requerido y debe ser válido.");

            // Validación de la fecha de ingreso
            RuleFor(x => x.EntryDate)
                .NotEmpty().WithMessage("La fecha de ingreso es requerida.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de ingreso no puede ser en el futuro.");

            // Validación del nivel de dependencia
            RuleFor(x => x.Id_DependencyLevel)
                .GreaterThan(0).WithMessage("El nivel de dependencia es requerido y debe ser válido.");

            // Validación de la localización
            RuleFor(x => x.Location_RD)
                .NotEmpty().WithMessage("La localización del residente es requerida.")
                .MaximumLength(250).WithMessage("La localización no puede exceder los 250 caracteres.");
        }
    }
}
