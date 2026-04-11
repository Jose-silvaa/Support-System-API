using FluentValidation;
using Support_System_API.Dtos.Ticket;

namespace Support_System_API.Validators.Ticket;

public class CreateTicketValidator : AbstractValidator<CreateTicketDto>
{
   public CreateTicketValidator()
   {
       RuleFor(x => x.Title)
           .NotEmpty()
           .MinimumLength(3);
   }   
}