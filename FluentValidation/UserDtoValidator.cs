using FluentValidation;
using guessing_game_backend.Dto;

namespace guessing_game_backend.FluentValidation
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required");
               

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                .Must(BeValidPassword).WithMessage("Invalid password format, Password must contain one Uppercase, one lawercase , number and symbol !");
        }

        private bool BeValidName(string name)
        {
            // Add custom validation logic for the name (not a number, not a char, etc.)
            return !string.IsNullOrWhiteSpace(name) && !name.Any(char.IsDigit) && !name.All(char.IsLetter);
        }

        private bool BeValidPassword(string password)
        {
            // Add custom validation logic for the password (uppercase, lowercase, number, char)
            return !string.IsNullOrWhiteSpace(password) &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(char.IsLetter);
        }
    }

}
