using CrmTask.Application.Tasks;
using CrmTask.Domain.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CrmTask.Infrastructure.Tasks;

public class TaskRepository(CrmDbContext dbContext) : ITaskRepository
{
    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        await dbContext.Tasks.AddAsync(task, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(Guid? customerId, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Tasks.AsNoTracking().Include(t => t.ChecklistItems).Include(t => t.Referrals).AsQueryable();

        if (customerId.HasValue)
        {
            query = query.Where(t => t.CustomerId == customerId.Value);
        }

        return await query.OrderBy(t => t.DueAt).ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Tasks
            .Include(t => t.ChecklistItems)
            .Include(t => t.Referrals)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // TaskItem.ReplaceChecklist() and Refer() add brand-new ChecklistItem/TaskReferral
        // instances (with a client-generated Guid Id already set, per their Create()
        // factories) into an *already-persisted* task's tracked collections. EF Core's
        // default heuristic for "is this Added or Modified" assumes a non-default key
        // means the row already exists, so it emits an UPDATE — which matches zero rows
        // and throws DbUpdateConcurrencyException. Same fix as CustomerRepository's
        // SaveChangesAsync for the identical ReplacePersonnel situation:
        // GetDatabaseValuesAsync is EF's own built-in "does a row with this key actually
        // exist" check; if not, it's a genuine insert, not an update.
        var modifiedOwnedEntries = dbContext.ChangeTracker.Entries<ChecklistItem>()
            .Where(e => e.State == EntityState.Modified)
            .Cast<EntityEntry>()
            .Concat(dbContext.ChangeTracker.Entries<TaskReferral>().Where(e => e.State == EntityState.Modified).Cast<EntityEntry>())
            .ToList();

        foreach (var entry in modifiedOwnedEntries)
        {
            if (await entry.GetDatabaseValuesAsync(cancellationToken) is null)
            {
                entry.State = EntityState.Added;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
