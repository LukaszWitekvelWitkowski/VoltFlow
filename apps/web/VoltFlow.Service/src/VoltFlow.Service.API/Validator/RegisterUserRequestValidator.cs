using FluentValidation;
using VoltFlow.Service.Application.Commands.Auth;
using VoltFlow.Service.Core.Models.Auth;

namespace VoltFlow.Service.API.Validator
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress().WithMessage("Podaj poprawny adres e-mail.");

            RuleFor(x => x.Password)
                .NotEmpty().MinimumLength(8).WithMessage("Hasło musi mieć min. 8 znaków.")
                .Matches(@"[A-Z]").WithMessage("Hasło musi mieć dużą literę.")
                .Matches(@"[0-9]").WithMessage("Hasło musi mieć cyfrę.")
                .Matches(@"[\!\?\*\.]").WithMessage("Hasło musi mieć znak specjalny (!?*.).");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Hasła muszą być identyczne.");
        }
    }
}
