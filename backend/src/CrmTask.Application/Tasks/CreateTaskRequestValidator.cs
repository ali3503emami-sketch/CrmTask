using FluentValidation;

namespace CrmTask.Application.Tasks;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(r => r.AssignedToStaffId)
            .NotEqual(Guid.Empty)
            .WithMessage("A task must be assigned to a staff member.");

        RuleForEach(r => r.ChecklistFields).SetValidator(new ChecklistFieldDefinitionValidator());
    }
}
