using FluentValidation;
using Support_System_API.Dtos.Auth;

namespace Support_System_API.Validators.User;

public class LoginUserValidator : AbstractValidator<LoginDto>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}