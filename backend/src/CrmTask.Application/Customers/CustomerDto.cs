using CrmTask.Domain.Customers;

namespace CrmTask.Application.Customers;

public record CustomerDto(Guid Id, string Name, CustomerCategory Category, string Phone, DateTimeOffset CreatedAt);
