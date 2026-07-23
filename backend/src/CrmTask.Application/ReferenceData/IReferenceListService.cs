using CrmTask.Domain.ReferenceData;

namespace CrmTask.Application.ReferenceData;

public interface IReferenceListService
{
    Task<ReferenceListItemDto> CreateAsync(ReferenceListKind kind, CreateReferenceListItemRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReferenceListItemDto>> GetByKindAsync(ReferenceListKind kind, CancellationToken cancellationToken = default);
}
