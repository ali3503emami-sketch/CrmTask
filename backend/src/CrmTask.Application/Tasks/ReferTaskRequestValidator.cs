using FluentValidation;

namespace CrmTask.Application.Tasks;

public class ReferTaskRequestValidator : AbstractValidator<ReferTaskRequest>
{
    public ReferTaskRequestValidator()
    {
        RuleFor(r => r.ReferredByStaffId)
            .NotEqual(Guid.Empty);

        RuleFor(r => r.ReferredToStaffId)
            .NotEqual(Guid.Empty)
            .WithMessage("A referral must name who it's referred to.");

        RuleFor(r => r.Note)
            .NotEmpty()
            .WithMessage("Referral note is required.");
    }
}
