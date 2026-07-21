using CrmTask.Application.Staff;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CrmTask.Api.Controllers;

[ApiController]
[Route("api/staff")]
public class StaffController(IStaffService staffService, IValidator<CreateStaffMemberRequest> validator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StaffMemberDto>>> GetActive(CancellationToken cancellationToken)
    {
        var staff = await staffService.GetActiveAsync(cancellationToken);
        return Ok(staff);
    }

    [HttpPost]
    public async Task<ActionResult<StaffMemberDto>> Create(CreateStaffMemberRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }

        var staffMember = await staffService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetActive), staffMember);
    }
}
