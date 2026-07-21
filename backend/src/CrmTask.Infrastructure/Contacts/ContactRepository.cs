using CrmTask.Application.Contacts;
using CrmTask.Domain.Contacts;
using Microsoft.EntityFrameworkCore;

namespace CrmTask.Infrastructure.Contacts;

public class ContactRepository(CrmDbContext dbContext) : IContactRepository
{
    public async Task AddAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        await dbContext.Contacts.AddAsync(contact, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Contact>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Contacts
            .AsNoTracking()
            .Where(c => c.CustomerId == customerId)
            .OrderByDescending(c => c.ContactedAt)
            .ToListAsync(cancellationToken);
    }
}
