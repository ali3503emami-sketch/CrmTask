using CrmTask.Domain.Customers;

namespace CrmTask.Application.Customers;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Same lookup as <see cref="GetByIdAsync"/> but change-tracked, for callers
    /// that mutate the entity and then call <see cref="SaveChangesAsync"/> — see
    /// <c>ITaskRepository.GetByIdAsync</c> for the same pattern explained.
    /// </summary>
    Task<Customer?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
