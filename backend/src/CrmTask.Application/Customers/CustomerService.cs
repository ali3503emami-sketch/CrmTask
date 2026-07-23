using CrmTask.Domain.Customers;
using Mapster;

namespace CrmTask.Application.Customers;

public class CustomerService(ICustomerRepository repository) : ICustomerService
{
    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = Customer.Create(request.Name, request.Category, request.Phone);

        await repository.AddAsync(customer, cancellationToken);

        return customer.Adapt<CustomerDto>();
    }

    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await repository.GetAllAsync(cancellationToken);

        return customers.Adapt<IReadOnlyList<CustomerDto>>();
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdAsync(id, cancellationToken);

        return customer?.Adapt<CustomerDto>();
    }

    public async Task<CustomerDto?> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetTrackedByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            return null;
        }

        customer.UpdateCore(request.Name, request.Category, request.Phone);
        customer.UpdateProfile(
            request.ManagerName,
            request.ManagerBirthDate,
            request.Address,
            request.Fax,
            request.Notes,
            request.NationalId,
            request.CategoryTitle,
            request.ActivityField);
        customer.ReplacePersonnel(request.Personnel.Select(p =>
            CustomerPersonnel.Create(p.FullName, p.Position, p.Phone, p.Mobile, p.Email)));

        await repository.SaveChangesAsync(cancellationToken);

        return customer.Adapt<CustomerDto>();
    }
}
