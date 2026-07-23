using CrmTask.Domain.Customers;

namespace CrmTask.Application.Customers;

public record CustomerDto(
    Guid Id,
    string Name,
    CustomerCategory Category,
    string Phone,
    DateTimeOffset CreatedAt,
    string CreatedAtShamsi,
    string? ManagerName,
    DateOnly? ManagerBirthDate,
    string? ManagerBirthDateShamsi,
    string? Address,
    string? Fax,
    string? Notes,
    string? NationalId,
    string? CategoryTitle,
    string? ActivityField,
    IReadOnlyList<CustomerPersonnelDto> Personnel);
