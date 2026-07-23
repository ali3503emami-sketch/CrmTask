namespace CrmTask.Application.Tasks;

public record CreateTaskRequest(
    string Title,
    string Description,
    DateTimeOffset DueAt,
    Guid? CustomerId,
    Guid AssignedToStaffId,
    Guid CreatedByStaffId,
    IReadOnlyList<ChecklistFieldDefinition> ChecklistFields);
