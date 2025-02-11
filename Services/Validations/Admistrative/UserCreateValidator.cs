using FluentValidation;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;

public class UserCreateValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateValidator()
    {
        RuleFor(user => user.Dni)
            .GreaterThan(0).WithMessage("El DNI debe ser un número válido.");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.");

        RuleFor(user => user.FullName)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.")
            .MinimumLength(3).WithMessage("El nombre completo debe tener al menos 3 caracteres.");

    }
}
