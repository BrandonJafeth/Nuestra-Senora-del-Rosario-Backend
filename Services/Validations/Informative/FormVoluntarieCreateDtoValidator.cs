using FluentValidation;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Validations.Informative
{
    public class FormVoluntarieCreateDtoValidator : AbstractValidator<FormVoluntarieCreateDto>
    {
        public FormVoluntarieCreateDtoValidator()
        {
            // Validación del nombre
            RuleFor(x => x.Vn_Name)
                .NotEmpty().WithMessage("El nombre es requerido.");

            // Validación del primer apellido
            RuleFor(x => x.Vn_Lastname1)
                .NotEmpty().WithMessage("El primer apellido es requerido.");

            // Validación de la cédula: debe tener 9 dígitos
            RuleFor(x => x.Vn_Cedula)
                .InclusiveBetween(100000000, 999999999)
                .WithMessage("La cédula debe ser un número de 9 dígitos.");

            // Validación del correo electrónico
            RuleFor(x => x.Vn_Email)
                .NotEmpty().WithMessage("El correo electrónico es requerido.")
                .EmailAddress().WithMessage("Un correo electrónico válido es requerido.");

            // Validación del teléfono: exactamente 8 dígitos
            RuleFor(x => x.Vn_Phone)
                .NotEmpty().WithMessage("El teléfono es requerido.")
                .Matches(@"^\d{8}$").WithMessage("El teléfono debe contener 8 dígitos.");

            // La fecha de inicio (Delivery_Date) no puede ser en el pasado
            RuleFor(x => x.Delivery_Date)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("La fecha de inicio no puede ser en el pasado.");

            // La fecha final (End_Date) debe ser posterior a la fecha de inicio
            RuleFor(x => x.End_Date)
                .GreaterThan(x => x.Delivery_Date)
                .WithMessage("La fecha final debe ser posterior a la fecha de inicio.");

            // Validación del tipo de voluntariado: debe ser mayor que cero
            RuleFor(x => x.VoluntarieTypeId)
                .GreaterThan(0).WithMessage("El tipo de voluntariado es requerido.");
        }
    }
}