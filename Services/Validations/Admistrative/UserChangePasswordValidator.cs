using FluentValidation;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;

public class UserChangePasswordValidator : AbstractValidator<UserChangePasswordDto>
{
    public UserChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("La contraseña actual es obligatoria.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("La nueva contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La nueva contraseña debe tener al menos 8 caracteres.")
            .Matches("[A-Z]").WithMessage("La nueva contraseña debe contener al menos una letra mayúscula.")
            .Matches("[a-z]").WithMessage("La nueva contraseña debe contener al menos una letra minúscula.")
            .Matches("[0-9]").WithMessage("La nueva contraseña debe contener al menos un número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("La nueva contraseña debe contener al menos un carácter especial.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage("Las contraseñas no coinciden.");
    }
}
