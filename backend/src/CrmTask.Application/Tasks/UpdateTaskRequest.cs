namespace CrmTask.Application.Tasks;

/// <summary>
/// <paramref name="RequestedByStaffId"/> is checked against the task's
/// <c>CreatedByStaffId</c> by <see cref="TaskService.UpdateAsync"/> — only the
/// creator may edit a task.
/// </summary>
public record UpdateTaskRequest(
    string Title,
    string Description,
    DateTimeOffset DueAt,
    Guid? CustomerId,
    Guid AssignedToStaffId,
    Guid RequestedByStaffId,
    IReadOnlyList<ChecklistFieldDefinition> ChecklistFields);
