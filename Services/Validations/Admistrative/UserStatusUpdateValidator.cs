using FluentValidation;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;

public class UserStatusUpdateValidator : AbstractValidator<UserStatusUpdateDto>
{
    public UserStatusUpdateValidator()
    {
        RuleFor(x => x.IsActive)
            .NotNull().WithMessage("El estado del usuario es obligatorio.");
    }
}
