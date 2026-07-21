namespace CrmTask.Application.Tasks;

public record CreateTaskRequest(
    string Title,
    string Description,
    DateTimeOffset DueAt,
    Guid? CustomerId,
    Guid AssignedToStaffId,
    IReadOnlyList<ChecklistFieldDefinition> ChecklistFields);
