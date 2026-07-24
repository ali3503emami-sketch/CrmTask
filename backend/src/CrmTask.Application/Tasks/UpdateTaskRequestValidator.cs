using FluentValidation;

namespace CrmTask.Application.Tasks;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(r => r.AssignedToStaffId)
            .NotEqual(Guid.Empty)
            .WithMessage("A task must be assigned to a staff member.");

        RuleFor(r => r.RequestedByStaffId)
            .NotEqual(Guid.Empty);

        RuleForEach(r => r.ChecklistFields).SetValidator(new ChecklistFieldDefinitionValidator());
    }
}
