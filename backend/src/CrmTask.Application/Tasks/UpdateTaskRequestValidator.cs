using FluentValidation;

namespace CrmTask.Application.Tasks;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty()
            .MaximumLength(300);
    }
}
