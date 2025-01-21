using System;
using FluentValidation;
using Infrastructure.Services.Informative.DTOS.CreatesDto;

namespace Infrastructure.Services.Informative.Validators
{
    public class FormDonationCreateDtoValidator : AbstractValidator<FormDonationCreateDto>
    {
        public FormDonationCreateDtoValidator()
        {
            RuleFor(x => x.Dn_Name)
                .NotEmpty().WithMessage("El nombre es requerido.");

            RuleFor(x => x.Dn_Lastname1)
                .NotEmpty().WithMessage("El apellido es requerido.");

            // Validación para que la cédula sea un número de 9 dígitos (100000000 a 999999999)
            RuleFor(x => x.Dn_Cedula)
                .InclusiveBetween(100000000, 999999999)
                .WithMessage("La cédula debe ser un número de 9 dígitos.");

            RuleFor(x => x.Dn_Email)
                .NotEmpty().EmailAddress().WithMessage("Un correo electrónico válido es requerido.");

            // Validación para que el teléfono contenga exactamente 8 dígitos
            RuleFor(x => x.Dn_Phone)
                .NotEmpty().WithMessage("El teléfono es requerido.")
                .Matches(@"^\d{8}$").WithMessage("El teléfono debe contener 8 dígitos.");

            // La fecha de entrega no puede ser en el pasado (permitiendo la fecha actual o futura)
            RuleFor(x => x.Delivery_date)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("La fecha de entrega no puede ser en el pasado.");

            RuleFor(x => x.Id_DonationType)
                .GreaterThan(0).WithMessage("El tipo de donación es requerido.");

            RuleFor(x => x.Id_MethodDonation)
                .GreaterThan(0).WithMessage("El método de donación es requerido.");
        }
    }
}
