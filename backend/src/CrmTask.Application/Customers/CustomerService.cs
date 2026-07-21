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
}
