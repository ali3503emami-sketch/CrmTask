using CrmTask.Domain.Contracts;

namespace CrmTask.Application.Contracts;

public interface IContractRepository
{
    Task AddAsync(Contract contract, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Contract>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}
