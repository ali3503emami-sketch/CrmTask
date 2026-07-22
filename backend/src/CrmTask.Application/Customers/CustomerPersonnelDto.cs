namespace CrmTask.Application.Customers;

public record CustomerPersonnelDto(Guid Id, string FullName, string? Position, string? Phone, string? Mobile, string? Email);
