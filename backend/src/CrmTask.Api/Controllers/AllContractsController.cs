using CrmTask.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CrmTask.Api.Controllers;

/// <summary>
/// Cross-customer contract listing — used by the Dashboard's "ended contracts"
/// tab. Contract creation stays nested under a customer (see <see cref="ContractsController"/>);
/// this is read-only.
/// </summary>
[ApiController]
[Route("api/contracts")]
public class AllContractsController(IContractService contractService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ContractDto>>> GetAll(CancellationToken cancellationToken)
    {
        var contracts = await contractService.GetAllAsync(cancellationToken);
        return Ok(contracts);
    }
}
