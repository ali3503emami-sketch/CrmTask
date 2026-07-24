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
            request.CreatedByStaffId,
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

        if (task.CreatedByStaffId != request.RequestedByStaffId)
        {
            throw new TaskAuthorizationException("Only the staff member who created this task may edit it.");
        }

        task.Update(request.Title, request.Description, request.DueAt, request.CustomerId, request.AssignedToStaffId);
        var checklistItems = request.ChecklistFields.Select(f => ChecklistItem.Create(f.Label, f.FieldType, f.Options)).ToList();
        task.ReplaceChecklist(checklistItems);
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

    public async Task<TaskItemDto?> ReferAsync(Guid taskId, ReferTaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = await repository.GetByIdAsync(taskId, cancellationToken);
        if (task is null)
        {
            return null;
        }

        if (!task.CanRefer(request.ReferredByStaffId))
        {
            throw new TaskAuthorizationException("Only the assignee or a previous referral recipient may refer this task.");
        }

        task.Refer(request.ReferredByStaffId, request.ReferredToStaffId, request.Note);
        await repository.SaveChangesAsync(cancellationToken);

        return ToDto(task);
    }

    private static TaskItemDto ToDto(TaskItem task)
    {
        var checklistItems = task.ChecklistItems
            .Select(i => new ChecklistItemDto(i.Id, i.Label, i.FieldType, i.Options, i.Value))
            .ToList();

        var referrals = task.Referrals
            .Select(r => new TaskReferralDto(r.Id, r.ReferredByStaffId, r.ReferredToStaffId, r.Note, r.ReferredAtShamsi))
            .ToList();

        return new TaskItemDto(
            task.Id,
            task.Title,
            task.Description,
            task.DueAt,
            task.DueAtShamsi,
            task.CustomerId,
            task.AssignedToStaffId,
            task.CreatedByStaffId,
            task.Status,
            checklistItems,
            referrals);
    }
}
