using CrmTask.Application.Contacts;
using CrmTask.Application.Customers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CrmTask.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId:guid}/contacts")]
public class ContactsController(
    IContactService contactService,
    ICustomerService customerService,
    IValidator<CreateContactRequest> validator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ContactDto>>> GetByCustomer(Guid customerId, CancellationToken cancellationToken)
    {
        if (await customerService.GetByIdAsync(customerId, cancellationToken) is null)
        {
            return NotFound();
        }

        var contacts = await contactService.GetByCustomerIdAsync(customerId, cancellationToken);
        return Ok(contacts);
    }

    [HttpPost]
    public async Task<ActionResult<ContactDto>> Create(Guid customerId, CreateContactRequest request, CancellationToken cancellationToken)
    {
        if (await customerService.GetByIdAsync(customerId, cancellationToken) is null)
        {
            return NotFound();
        }

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }

        var contact = await contactService.CreateAsync(customerId, request, cancellationToken);
        return CreatedAtAction(nameof(GetByCustomer), new { customerId }, contact);
    }
}
