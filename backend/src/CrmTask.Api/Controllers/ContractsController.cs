using CrmTask.Application.Contracts;
using CrmTask.Application.Customers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CrmTask.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId:guid}/contracts")]
public class ContractsController(
    IContractService contractService,
    ICustomerService customerService,
    IValidator<CreateContractRequest> validator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ContractDto>>> GetByCustomer(Guid customerId, CancellationToken cancellationToken)
    {
        if (await customerService.GetByIdAsync(customerId, cancellationToken) is null)
        {
            return NotFound();
        }

        var contracts = await contractService.GetByCustomerIdAsync(customerId, cancellationToken);
        return Ok(contracts);
    }

    [HttpPost]
    public async Task<ActionResult<ContractDto>> Create(Guid customerId, CreateContractRequest request, CancellationToken cancellationToken)
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

        var contract = await contractService.CreateAsync(customerId, request, cancellationToken);
        return CreatedAtAction(nameof(GetByCustomer), new { customerId }, contract);
    }
}
