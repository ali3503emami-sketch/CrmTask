using CrmTask.Domain.Tasks;

namespace CrmTask.Application.Tasks;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetAllAsync(Guid? customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a change-tracked instance (unlike other repositories' read methods,
    /// which return no-tracking snapshots) — callers mutate it in place and then
    /// call <see cref="SaveChangesAsync"/> to persist, matching how the rest of
    /// the mutation methods on <see cref="Domain.Tasks.TaskItem"/> work.
    /// </summary>
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
