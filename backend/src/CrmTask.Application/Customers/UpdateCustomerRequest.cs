using CrmTask.Domain.Customers;

namespace CrmTask.Application.Customers;

public record UpdateCustomerRequest(
    string Name,
    CustomerCategory Category,
    string Phone,
    string? ManagerName,
    DateOnly? ManagerBirthDate,
    string? Address,
    string? Fax,
    string? Notes,
    string? NationalId,
    IReadOnlyList<CustomerPersonnelInput> Personnel);
