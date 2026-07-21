using CrmTask.Domain.Contacts;
using Mapster;

namespace CrmTask.Application.Contacts;

public class ContactService(IContactRepository repository) : IContactService
{
    public async Task<ContactDto> CreateAsync(Guid customerId, CreateContactRequest request, CancellationToken cancellationToken = default)
    {
        var contact = Contact.Create(customerId, request.Direction, request.Summary, request.ContactedAt, request.NextFollowUpAt);

        await repository.AddAsync(contact, cancellationToken);

        return contact.Adapt<ContactDto>();
    }

    public async Task<IReadOnlyList<ContactDto>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var contacts = await repository.GetByCustomerIdAsync(customerId, cancellationToken);

        return contacts.Adapt<IReadOnlyList<ContactDto>>();
    }
}
