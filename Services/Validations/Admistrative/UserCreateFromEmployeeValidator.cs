using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Infrastructure.Validations.Admistrative
{
 
    public class UserCreateFromEmployeeValidator : AbstractValidator<(int dniEmployee, int idRole)>
    {
        public UserCreateFromEmployeeValidator()
        {
            RuleFor(x => x.dniEmployee)
                .GreaterThan(0).WithMessage("El DNI del empleado debe ser un número válido.");

            RuleFor(x => x.idRole)
                .GreaterThan(0).WithMessage("El ID del rol debe ser un número válido.");
        }
    }

}
