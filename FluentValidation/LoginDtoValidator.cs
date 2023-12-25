
using FluentValidation;
using guessing_game_backend.Dto;

namespace guessing_game_backend.FluentValidation
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                              .Must(BeValidPassword).WithMessage("Invalid password format, Password must contain one Uppercase, one lawercase , number and symbol !");

        }

        private bool BeValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(char.IsLetter);
        }
    }

}
