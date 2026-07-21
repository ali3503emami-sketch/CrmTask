using CrmTask.Domain.Tasks;

namespace CrmTask.Application.Tasks;

public record ChecklistFieldDefinition(string Label, ChecklistFieldType FieldType, IReadOnlyList<string>? Options);
