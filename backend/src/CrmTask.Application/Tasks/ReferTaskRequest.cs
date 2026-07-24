namespace CrmTask.Application.Tasks;

public record ReferTaskRequest(Guid ReferredByStaffId, Guid ReferredToStaffId, string Note);
