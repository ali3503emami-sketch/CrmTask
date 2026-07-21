using CrmTask.Application.Tasks;
using CrmTask.Domain.Tasks;
using Microsoft.EntityFrameworkCore;

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
        var query = dbContext.Tasks.AsNoTracking().Include(t => t.ChecklistItems).AsQueryable();

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
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
