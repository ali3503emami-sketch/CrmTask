using CrmTask.Domain.Contracts;

namespace CrmTask.Application.Contracts;

public class ContractService(IContractRepository repository, TimeProvider timeProvider) : IContractService
{
    public async Task<ContractDto> CreateAsync(Guid customerId, CreateContractRequest request, CancellationToken cancellationToken = default)
    {
        var contract = Contract.Create(customerId, request.Title, request.Amount, request.StartDate, request.EndDate);

        await repository.AddAsync(contract, cancellationToken);

        return ToDto(contract);
    }

    public async Task<IReadOnlyList<ContractDto>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var contracts = await repository.GetByCustomerIdAsync(customerId, cancellationToken);

        return contracts.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<ContractDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var contracts = await repository.GetAllAsync(cancellationToken);

        return contracts.Select(ToDto).ToList();
    }

    private ContractDto ToDto(Contract contract)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        return new ContractDto(
            contract.Id,
            contract.CustomerId,
            contract.Title,
            contract.Amount,
            contract.StartDate,
            contract.StartDateShamsi,
            contract.EndDate,
            contract.EndDateShamsi,
            contract.GetStatus(today));
    }
}
