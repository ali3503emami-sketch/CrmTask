using FluentValidation;

namespace CrmTask.Application.Customers;

public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(r => r.Phone)
            .NotEmpty()
            .Matches(@"^[0-9+\-\s]{7,20}$")
            .WithMessage("Phone must be 7-20 digits, optionally with +, -, or spaces.");
    }
}
