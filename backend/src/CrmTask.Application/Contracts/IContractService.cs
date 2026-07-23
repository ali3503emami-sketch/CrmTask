namespace CrmTask.Application.Contracts;

public interface IContractService
{
    Task<ContractDto> CreateAsync(Guid customerId, CreateContractRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContractDto>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContractDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
