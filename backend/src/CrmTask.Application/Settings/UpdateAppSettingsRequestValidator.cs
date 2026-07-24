using FluentValidation;

namespace CrmTask.Application.Settings;

public class UpdateAppSettingsRequestValidator : AbstractValidator<UpdateAppSettingsRequest>
{
    public UpdateAppSettingsRequestValidator()
    {
        RuleFor(r => r.TaskUpcomingWindowDays)
            .GreaterThanOrEqualTo(0);

        RuleFor(r => r.ContractEndingWindowDays)
            .GreaterThanOrEqualTo(0);
    }
}
