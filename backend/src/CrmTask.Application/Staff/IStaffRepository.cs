using CrmTask.Domain.Staff;

namespace CrmTask.Application.Staff;

public interface IStaffRepository
{
    Task AddAsync(StaffMember staffMember, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StaffMember>> GetActiveAsync(CancellationToken cancellationToken = default);

    Task<StaffMember?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
