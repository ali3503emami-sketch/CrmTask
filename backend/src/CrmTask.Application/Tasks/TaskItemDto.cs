using CrmTask.Domain.Tasks;

namespace CrmTask.Application.Tasks;

public record TaskItemDto(
    Guid Id,
    string Title,
    string Description,
    DateTimeOffset DueAt,
    string DueAtShamsi,
    Guid? CustomerId,
    Guid AssignedToStaffId,
    Guid CreatedByStaffId,
    TaskItemStatus Status,
    IReadOnlyList<ChecklistItemDto> ChecklistItems);
