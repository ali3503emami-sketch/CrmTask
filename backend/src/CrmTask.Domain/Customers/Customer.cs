using CrmTask.Domain.Shared;

namespace CrmTask.Domain.Customers;

/// <summary>
/// A customer of record — the central entity most other modules (contracts,
/// tasks, service requests) attach to.
/// </summary>
public class Customer
{
    private readonly List<CustomerPersonnel> _personnel = [];

    private Customer()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    public CustomerCategory Category { get; private set; }

    public string Phone { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    public string CreatedAtShamsi { get; private set; } = null!;

    public string? ManagerName { get; private set; }

    public DateOnly? ManagerBirthDate { get; private set; }

    public string? ManagerBirthDateShamsi { get; private set; }

    public string? Address { get; private set; }

    public string? Fax { get; private set; }

    public string? Notes { get; private set; }

    /// <summary>
    /// شماره ملی (individual) or شناسه ملی (legal entity) — one shared field
    /// regardless of <see cref="Category"/>, per product decision.
    /// </summary>
    public string? NationalId { get; private set; }

    public IReadOnlyList<CustomerPersonnel> Personnel => _personnel;

    public static Customer Create(string name, CustomerCategory category, string phone)
    {
        var createdAt = DateTimeOffset.UtcNow;
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            CreatedAt = createdAt,
            CreatedAtShamsi = PersianDateConverter.ToShamsi(createdAt),
        };
        customer.UpdateCore(name, category, phone);

        return customer;
    }

    public void UpdateCore(string name, CustomerCategory category, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Customer name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("Customer phone is required.", nameof(phone));
        }

        Name = name.Trim();
        Category = category;
        Phone = phone.Trim();
    }

    public void UpdateProfile(
        string? managerName,
        DateOnly? managerBirthDate,
        string? address,
        string? fax,
        string? notes,
        string? nationalId)
    {
        ManagerName = string.IsNullOrWhiteSpace(managerName) ? null : managerName.Trim();
        ManagerBirthDate = managerBirthDate;
        ManagerBirthDateShamsi = managerBirthDate.HasValue ? PersianDateConverter.ToShamsi(managerBirthDate.Value) : null;
        Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
        Fax = string.IsNullOrWhiteSpace(fax) ? null : fax.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        NationalId = string.IsNullOrWhiteSpace(nationalId) ? null : nationalId.Trim();
    }

    public void ReplacePersonnel(IEnumerable<CustomerPersonnel> personnel)
    {
        _personnel.Clear();
        _personnel.AddRange(personnel);
    }
}
