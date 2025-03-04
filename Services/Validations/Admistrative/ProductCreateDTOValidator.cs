using FluentValidation;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Validations.Admistrative
{
    public class ProductCreateDTOValidator : AbstractValidator<ProductCreateDTO>
    {
        public ProductCreateDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(100).WithMessage("El nombre no debe exceder 100 caracteres.");

            RuleFor(x => x.CategoryID)
                .GreaterThan(0).WithMessage("La categoría es obligatoria y debe ser válida.");

            RuleFor(x => x.UnitOfMeasureID)
                .GreaterThan(0).WithMessage("La unidad de medida es obligatoria y debe ser válida.");

            RuleFor(x => x.InitialQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad inicial debe ser mayor o igual a cero.");
        }
    }
}
