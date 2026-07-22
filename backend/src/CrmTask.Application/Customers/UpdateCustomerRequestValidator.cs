using FluentValidation;

namespace CrmTask.Application.Customers;

public class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerRequestValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(r => r.Phone)
            .NotEmpty()
            .Matches(@"^[0-9+\-\s]{7,20}$");

        RuleForEach(r => r.Personnel).SetValidator(new CustomerPersonnelInputValidator());
    }
}
