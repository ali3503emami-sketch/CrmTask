using CrmTask.Domain.ReferenceData;

namespace CrmTask.Application.ReferenceData;

public interface IReferenceListRepository
{
    Task AddAsync(ReferenceListItem item, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReferenceListItem>> GetByKindAsync(ReferenceListKind kind, CancellationToken cancellationToken = default);

    /// <summary>
    /// Same lookup as a by-id GET would be, but change-tracked, for callers that
    /// mutate the entity and then call <see cref="SaveChangesAsync"/> — see
    /// <c>ICustomerRepository.GetTrackedByIdAsync</c> for the same pattern.
    /// </summary>
    Task<ReferenceListItem?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
