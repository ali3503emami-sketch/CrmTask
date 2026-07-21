using FluentValidation;

namespace CrmTask.Application.Staff;

public class CreateStaffMemberRequestValidator : AbstractValidator<CreateStaffMemberRequest>
{
    public CreateStaffMemberRequestValidator()
    {
        RuleFor(r => r.FullName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(r => r.PhoneNumber)
            .NotEmpty()
            .Matches(@"^[0-9+\-\s]{7,20}$");
    }
}
