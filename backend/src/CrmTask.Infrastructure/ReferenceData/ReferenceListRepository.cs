using CrmTask.Application.ReferenceData;
using CrmTask.Domain.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace CrmTask.Infrastructure.ReferenceData;

public class ReferenceListRepository(CrmDbContext dbContext) : IReferenceListRepository
{
    public async Task AddAsync(ReferenceListItem item, CancellationToken cancellationToken = default)
    {
        await dbContext.ReferenceListItems.AddAsync(item, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReferenceListItem>> GetByKindAsync(ReferenceListKind kind, CancellationToken cancellationToken = default)
    {
        return await dbContext.ReferenceListItems
            .AsNoTracking()
            .Where(i => i.Kind == kind)
            .OrderBy(i => i.Title)
            .ToListAsync(cancellationToken);
    }
}
