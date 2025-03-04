using FluentValidation;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Validations.Admistrative
{
    public class InventoryCreateDTOValidator : AbstractValidator<InventoryCreateDTO>
    {
        public InventoryCreateDTOValidator()
        {
            // ProductID debe ser mayor a 0.
            RuleFor(x => x.ProductID)
                .GreaterThan(0).WithMessage("El ID del producto debe ser mayor a cero.");

            // Quantity debe ser mayor a 0.
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.");

            // Date no puede estar en el futuro.
            RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha no puede ser en el futuro.");

            // MovementType es requerido y debe ser "Ingreso" o "Egreso".
            RuleFor(x => x.MovementType)
                .NotEmpty().WithMessage("El tipo de movimiento es requerido.")
                .Must(mt => mt == "Ingreso" || mt == "Egreso")
                    .WithMessage("El tipo de movimiento debe ser 'Ingreso' o 'Egreso'.");
        }
    }
}
