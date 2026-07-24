using CrmTask.Application.Staff;
using CrmTask.Domain.Staff;
using Microsoft.EntityFrameworkCore;

namespace CrmTask.Infrastructure.Staff;

public class StaffRepository(CrmDbContext dbContext) : IStaffRepository
{
    public async Task AddAsync(StaffMember staffMember, CancellationToken cancellationToken = default)
    {
        await dbContext.StaffMembers.AddAsync(staffMember, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StaffMember>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.StaffMembers
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task<StaffMember?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<StaffMember?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.StaffMembers.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
