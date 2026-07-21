using CrmTask.Domain.Tasks;

namespace CrmTask.Application.Tasks;

public record ChecklistItemDto(Guid Id, string Label, ChecklistFieldType FieldType, IReadOnlyList<string> Options, string? Value);
