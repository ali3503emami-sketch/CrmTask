namespace CrmTask.Application.Tasks;

public interface ITaskService
{
    Task<TaskItemDto> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItemDto>> GetAllAsync(Guid? customerId, CancellationToken cancellationToken = default);

    Task<TaskItemDto?> GetByIdAsync(Guid taskId, CancellationToken cancellationToken = default);

    Task<TaskItemDto?> UpdateAsync(Guid taskId, UpdateTaskRequest request, CancellationToken cancellationToken = default);

    Task<TaskItemDto?> MarkAsDoneAsync(Guid taskId, CancellationToken cancellationToken = default);

    Task<TaskItemDto?> ReassignAsync(Guid taskId, Guid staffId, CancellationToken cancellationToken = default);

    Task<TaskItemDto?> SetChecklistItemValueAsync(Guid taskId, Guid checklistItemId, string? value, CancellationToken cancellationToken = default);
}
