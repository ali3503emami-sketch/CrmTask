using CrmTask.Domain.Staff;

namespace CrmTask.Application.Staff;

public interface IStaffRepository
{
    Task AddAsync(StaffMember staffMember, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StaffMember>> GetActiveAsync(CancellationToken cancellationToken = default);

    Task<StaffMember?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Same lookup as <see cref="GetByIdAsync"/> but change-tracked, for callers
    /// that mutate the entity and then call <see cref="SaveChangesAsync"/> — see
    /// <c>ICustomerRepository.GetTrackedByIdAsync</c> for the same pattern.
    /// </summary>
    Task<StaffMember?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
