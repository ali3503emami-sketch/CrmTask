using CrmTask.Domain.ReferenceData;

namespace CrmTask.Application.ReferenceData;

public class ReferenceListService(IReferenceListRepository repository) : IReferenceListService
{
    public async Task<ReferenceListItemDto> CreateAsync(ReferenceListKind kind, CreateReferenceListItemRequest request, CancellationToken cancellationToken = default)
    {
        var item = ReferenceListItem.Create(kind, request.Title);

        await repository.AddAsync(item, cancellationToken);

        return new ReferenceListItemDto(item.Id, item.Title);
    }

    public async Task<IReadOnlyList<ReferenceListItemDto>> GetByKindAsync(ReferenceListKind kind, CancellationToken cancellationToken = default)
    {
        var items = await repository.GetByKindAsync(kind, cancellationToken);

        return items.Select(i => new ReferenceListItemDto(i.Id, i.Title)).ToList();
    }
}
