using FluentValidation;

namespace CrmTask.Application.ReferenceData;

public class CreateReferenceListItemRequestValidator : AbstractValidator<CreateReferenceListItemRequest>
{
    public CreateReferenceListItemRequestValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty()
            .MaximumLength(200);
    }
}
