namespace CrmTask.Application.Tasks;

public record TaskReferralDto(Guid Id, Guid ReferredByStaffId, Guid ReferredToStaffId, string Note, string ReferredAtShamsi);
