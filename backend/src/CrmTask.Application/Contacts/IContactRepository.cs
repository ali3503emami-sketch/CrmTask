using CrmTask.Domain.Contacts;

namespace CrmTask.Application.Contacts;

public interface IContactRepository
{
    Task AddAsync(Contact contact, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Contact>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}
