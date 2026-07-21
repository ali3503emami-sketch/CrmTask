using CrmTask.Domain.Customers;

namespace CrmTask.Application.Customers;

public record CreateCustomerRequest(string Name, CustomerCategory Category, string Phone);
