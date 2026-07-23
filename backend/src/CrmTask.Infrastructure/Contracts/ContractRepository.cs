using CrmTask.Application.Contracts;
using CrmTask.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CrmTask.Infrastructure.Contracts;

public class ContractRepository(CrmDbContext dbContext) : IContractRepository
{
    public async Task AddAsync(Contract contract, CancellationToken cancellationToken = default)
    {
        await dbContext.Contracts.AddAsync(contract, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Contract>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Contracts
            .AsNoTracking()
            .Where(c => c.CustomerId == customerId)
            .OrderByDescending(c => c.EndDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Contract>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Contracts
            .AsNoTracking()
            .OrderByDescending(c => c.EndDate)
            .ToListAsync(cancellationToken);
    }
}
