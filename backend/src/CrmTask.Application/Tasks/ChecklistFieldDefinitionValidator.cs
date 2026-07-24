using CrmTask.Domain.Tasks;
using FluentValidation;

namespace CrmTask.Application.Tasks;

public class ChecklistFieldDefinitionValidator : AbstractValidator<ChecklistFieldDefinition>
{
    private static readonly ChecklistFieldType[] ChoiceFieldTypes = [ChecklistFieldType.Dropdown];

    public ChecklistFieldDefinitionValidator()
    {
        RuleFor(f => f.Label)
            .NotEmpty();

        RuleFor(f => f.Options)
            .Must(options => options is { Count: > 0 })
            .When(f => ChoiceFieldTypes.Contains(f.FieldType))
            .WithMessage("Dropdown fields require at least one option.");
    }
}
