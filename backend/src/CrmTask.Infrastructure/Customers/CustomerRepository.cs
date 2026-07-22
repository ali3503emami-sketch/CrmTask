using CrmTask.Application.Customers;
using CrmTask.Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace CrmTask.Infrastructure.Customers;

public class CustomerRepository(CrmDbContext dbContext) : ICustomerRepository
{
    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await dbContext.Customers.AddAsync(customer, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Customers
            .AsNoTracking()
            .Include(c => c.Personnel)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Customers
            .AsNoTracking()
            .Include(c => c.Personnel)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Customers
            .Include(c => c.Personnel)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Customer.ReplacePersonnel() creates brand-new CustomerPersonnel instances
        // (with a client-generated Guid Id already set, per Create()) and swaps them
        // into an *already-persisted* customer's tracked collection. EF Core's default
        // heuristic for "is this Added or Modified" assumes a non-default key means
        // the row already exists, so it emits an UPDATE — which matches zero rows and
        // throws DbUpdateConcurrencyException. GetDatabaseValuesAsync is EF's own
        // built-in "does a row with this key actually exist" check; if not, it's a
        // genuine insert, not an update.
        foreach (var entry in dbContext.ChangeTracker.Entries<CustomerPersonnel>().Where(e => e.State == EntityState.Modified).ToList())
        {
            if (await entry.GetDatabaseValuesAsync(cancellationToken) is null)
            {
                entry.State = EntityState.Added;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
