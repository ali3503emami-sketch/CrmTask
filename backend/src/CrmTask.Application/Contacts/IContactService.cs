namespace CrmTask.Application.Contacts;

public interface IContactService
{
    Task<ContactDto> CreateAsync(Guid customerId, CreateContactRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContactDto>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}
