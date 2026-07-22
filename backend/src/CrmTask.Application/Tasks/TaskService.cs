using CrmTask.Domain.Tasks;

namespace CrmTask.Application.Tasks;

public class TaskService(ITaskRepository repository) : ITaskService
{
    public async Task<TaskItemDto> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var checklistItems = request.ChecklistFields
            .Select(f => ChecklistItem.Create(f.Label, f.FieldType, f.Options))
            .ToList();

        var task = TaskItem.Create(
            request.Title,
            request.Description,
            request.DueAt,
            request.CustomerId,
            request.AssignedToStaffId,
            checklistItems);

        await repository.AddAsync(task, cancellationToken);

        return ToDto(task);
    }

    public async Task<IReadOnlyList<TaskItemDto>> GetAllAsync(Guid? customerId, CancellationToken cancellationToken = default)
    {
        var tasks = await repository.GetAllAsync(customerId, cancellationToken);

        return tasks.Select(ToDto).ToList();
    }

    public async Task<TaskItemDto?> GetByIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await repository.GetByIdAsync(taskId, cancellationToken);

        return task is null ? null : ToDto(task);
    }

    public async Task<TaskItemDto?> UpdateAsync(Guid taskId, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = await repository.GetByIdAsync(taskId, cancellationToken);
        if (task is null)
        {
            return null;
        }

        task.Update(request.Title, request.Description, request.DueAt, request.CustomerId);
        await repository.SaveChangesAsync(cancellationToken);

        return ToDto(task);
    }

    public async Task<TaskItemDto?> MarkAsDoneAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await repository.GetByIdAsync(taskId, cancellationToken);
        if (task is null)
        {
            return null;
        }

        task.MarkAsDone();
        await repository.SaveChangesAsync(cancellationToken);

        return ToDto(task);
    }

    public async Task<TaskItemDto?> ReassignAsync(Guid taskId, Guid staffId, CancellationToken cancellationToken = default)
    {
        var task = await repository.GetByIdAsync(taskId, cancellationToken);
        if (task is null)
        {
            return null;
        }

        task.Reassign(staffId);
        await repository.SaveChangesAsync(cancellationToken);

        return ToDto(task);
    }

    public async Task<TaskItemDto?> SetChecklistItemValueAsync(Guid taskId, Guid checklistItemId, string? value, CancellationToken cancellationToken = default)
    {
        var task = await repository.GetByIdAsync(taskId, cancellationToken);
        if (task is null)
        {
            return null;
        }

        task.SetChecklistItemValue(checklistItemId, value);
        await repository.SaveChangesAsync(cancellationToken);

        return ToDto(task);
    }

    private static TaskItemDto ToDto(TaskItem task)
    {
        var checklistItems = task.ChecklistItems
            .Select(i => new ChecklistItemDto(i.Id, i.Label, i.FieldType, i.Options, i.Value))
            .ToList();

        return new TaskItemDto(
            task.Id,
            task.Title,
            task.Description,
            task.DueAt,
            task.DueAtShamsi,
            task.CustomerId,
            task.AssignedToStaffId,
            task.Status,
            checklistItems);
    }
}
