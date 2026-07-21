using CrmTask.Domain.Staff;
using Mapster;

namespace CrmTask.Application.Staff;

public class StaffService(IStaffRepository repository) : IStaffService
{
    public async Task<StaffMemberDto> CreateAsync(CreateStaffMemberRequest request, CancellationToken cancellationToken = default)
    {
        var staffMember = StaffMember.Create(request.FullName, request.PhoneNumber);

        await repository.AddAsync(staffMember, cancellationToken);

        return staffMember.Adapt<StaffMemberDto>();
    }

    public async Task<IReadOnlyList<StaffMemberDto>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var staff = await repository.GetActiveAsync(cancellationToken);

        return staff.Adapt<IReadOnlyList<StaffMemberDto>>();
    }

    public async Task<StaffMemberDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var staffMember = await repository.GetByIdAsync(id, cancellationToken);

        return staffMember?.Adapt<StaffMemberDto>();
    }
}
