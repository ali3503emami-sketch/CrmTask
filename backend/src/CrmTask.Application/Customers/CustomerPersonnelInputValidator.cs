using FluentValidation;

namespace CrmTask.Application.Customers;

public class CustomerPersonnelInputValidator : AbstractValidator<CustomerPersonnelInput>
{
    public CustomerPersonnelInputValidator()
    {
        RuleFor(p => p.FullName)
            .NotEmpty()
            .MaximumLength(200);
    }
}
