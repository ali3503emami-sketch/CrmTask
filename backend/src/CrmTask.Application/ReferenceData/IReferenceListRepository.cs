using CrmTask.Domain.ReferenceData;

namespace CrmTask.Application.ReferenceData;

public interface IReferenceListRepository
{
    Task AddAsync(ReferenceListItem item, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReferenceListItem>> GetByKindAsync(ReferenceListKind kind, CancellationToken cancellationToken = default);
}
