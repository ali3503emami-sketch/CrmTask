namespace CrmTask.Application.Tasks;

/// <summary>
/// Thrown when a staff member attempts an action they're not permitted to
/// perform on a task — editing without being its creator, or referring it
/// without being the assignee or a past referral recipient. Mapped to
/// HTTP 403 by <see cref="Api.Controllers.TasksController"/>.
/// </summary>
public class TaskAuthorizationException(string message) : Exception(message);
