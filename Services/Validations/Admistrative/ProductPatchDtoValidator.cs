using FluentValidation;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Validations.Admistrative
{
    public class ProductPatchDtoValidator : AbstractValidator<ProductPatchDto>
    {
        public ProductPatchDtoValidator()
        {
            When(x => x.Name != null, () =>
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("El nombre no debe estar vacío.")
                    .MaximumLength(100).WithMessage("El nombre no debe exceder 100 caracteres.");
            });

            When(x => x.CategoryID.HasValue, () =>
            {
                RuleFor(x => x.CategoryID.Value)
                    .GreaterThan(0).WithMessage("La categoría debe ser válida.");
            });

            When(x => x.UnitOfMeasureID.HasValue, () =>
            {
                RuleFor(x => x.UnitOfMeasureID.Value)
                    .GreaterThan(0).WithMessage("La unidad de medida debe ser válida.");
            });

            When(x => x.TotalQuantity.HasValue, () =>
            {
                RuleFor(x => x.TotalQuantity.Value)
                    .GreaterThanOrEqualTo(0).WithMessage("La cantidad total debe ser mayor o igual a cero.");
            });
        }
    }

}