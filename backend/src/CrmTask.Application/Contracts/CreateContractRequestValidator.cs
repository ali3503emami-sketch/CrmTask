using FluentValidation;

namespace CrmTask.Application.Contracts;

public class CreateContractRequestValidator : AbstractValidator<CreateContractRequest>
{
    public CreateContractRequestValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(r => r.Amount)
            .GreaterThanOrEqualTo(0);

        RuleFor(r => r.EndDate)
            .GreaterThan(r => r.StartDate)
            .WithMessage("End date must be after the start date.");
    }
}
