namespace CrmTask.Application.Tasks;

public record UpdateTaskRequest(string Title, string Description, DateTimeOffset DueAt, Guid? CustomerId);
