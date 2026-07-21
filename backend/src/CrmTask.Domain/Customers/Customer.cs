namespace CrmTask.Domain.Customers;

/// <summary>
/// A customer of record — the central entity most other modules (contracts,
/// tasks, service requests) attach to.
/// </summary>
public class Customer
{
    private Customer()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    public CustomerCategory Category { get; private set; }

    public string Phone { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    public static Customer Create(string name, CustomerCategory category, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Customer name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("Customer phone is required.", nameof(phone));
        }

        return new Customer
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Category = category,
            Phone = phone.Trim(),
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
}
