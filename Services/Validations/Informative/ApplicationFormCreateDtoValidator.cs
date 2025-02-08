using FluentValidation;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using System;

namespace Infrastructure.Validations.Informative
{
    public class ApplicationFormCreateDtoValidator : AbstractValidator<ApplicationFormCreateDto>
    {
        public ApplicationFormCreateDtoValidator()
        {
            // Validación del nombre del aplicante
            RuleFor(x => x.Name_AP)
         .NotEmpty().WithMessage("El nombre del aplicante es requerido.")
         .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");


            // Validación del primer apellido del aplicante
            RuleFor(x => x.LastName1_AP)
                .NotEmpty().WithMessage("El primer apellido del aplicante es requerido.")
                .MaximumLength(100).WithMessage("El apellido no puede exceder los 100 caracteres.");

            // Validación de la cédula del aplicante
            RuleFor(x => x.Cedula_AP)
                .NotEmpty().WithMessage("La cédula del aplicante es requerida.")
                .Matches(@"^[0-9]{9,12}$").WithMessage("La cédula debe tener entre 9 y 12 dígitos.");

            // Validación de la edad del aplicante
            RuleFor(x => x.Age_AP)
                .GreaterThanOrEqualTo(65).WithMessage("La edad del aplicante debe ser mayor o igual a 65 años.");

            // Validación de la localización del aplicante
            RuleFor(x => x.Location_AP)
                .NotEmpty().WithMessage("La localización del aplicante es requerida.")
                .MaximumLength(255).WithMessage("La localización no puede exceder los 255 caracteres.");

            // Validación del nombre del guardián
            RuleFor(x => x.GuardianName)
                .NotEmpty().WithMessage("El nombre del guardián es requerido.")
                .MaximumLength(100).WithMessage("El nombre del guardián no puede exceder los 100 caracteres.");

            // Validación del primer apellido del guardián
            RuleFor(x => x.GuardianLastName1)
                .NotEmpty().WithMessage("El primer apellido del guardián es requerido.")
                .MaximumLength(100).WithMessage("El apellido del guardián no puede exceder los 100 caracteres.");

            // Validación de la cédula del guardián
            RuleFor(x => x.GuardianCedula)
                .NotEmpty().WithMessage("La cédula del guardián es requerida.")
                .Matches(@"^[0-9]{9,12}$").WithMessage("La cédula del guardián debe tener entre 9 y 12 dígitos.");

            // Validación del correo del guardián
            RuleFor(x => x.GuardianEmail)
                .NotEmpty().WithMessage("El correo del guardián es requerido.")
                .EmailAddress().WithMessage("Debe proporcionar un correo electrónico válido.");

            // Validación del teléfono del guardián
            RuleFor(x => x.GuardianPhone)
                .NotEmpty().WithMessage("El teléfono del guardián es requerido.")
                .Matches(@"^\d{8}$").WithMessage("El teléfono debe contener 8 dígitos.");
        }
    }
}
