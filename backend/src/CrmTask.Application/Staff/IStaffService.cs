namespace CrmTask.Application.Staff;

public interface IStaffService
{
    Task<StaffMemberDto> CreateAsync(CreateStaffMemberRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StaffMemberDto>> GetActiveAsync(CancellationToken cancellationToken = default);

    Task<StaffMemberDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
