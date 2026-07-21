using FluentValidation;

namespace CrmTask.Application.Contacts;

public class CreateContactRequestValidator : AbstractValidator<CreateContactRequest>
{
    public CreateContactRequestValidator()
    {
        RuleFor(r => r.Summary)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(r => r.NextFollowUpAt)
            .GreaterThan(r => r.ContactedAt)
            .When(r => r.NextFollowUpAt.HasValue)
            .WithMessage("Next follow-up date must be after the contact date.");
    }
}
