using FluentValidation;
using Support_System_API.Dtos;

namespace Support_System_API.Validators.User;

public class CreateUserValidator : AbstractValidator<RegisterDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}