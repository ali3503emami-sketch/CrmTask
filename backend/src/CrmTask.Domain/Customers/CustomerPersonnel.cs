namespace CrmTask.Domain.Customers;

/// <summary>
/// A contact person who works for a customer (the customer's own staff) —
/// distinct from <see cref="Contacts.Contact"/> (a logged call) and from our
/// own internal <see cref="Staff.StaffMember"/>.
/// </summary>
public class CustomerPersonnel
{
    private CustomerPersonnel()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    public string FullName { get; private set; } = null!;

    public string? Position { get; private set; }

    public string? Phone { get; private set; }

    public string? Mobile { get; private set; }

    public string? Email { get; private set; }

    public static CustomerPersonnel Create(string fullName, string? position, string? phone, string? mobile, string? email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Personnel full name is required.", nameof(fullName));
        }

        return new CustomerPersonnel
        {
            Id = Guid.NewGuid(),
            FullName = fullName.Trim(),
            Position = string.IsNullOrWhiteSpace(position) ? null : position.Trim(),
            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim(),
            Mobile = string.IsNullOrWhiteSpace(mobile) ? null : mobile.Trim(),
            Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
        };
    }
}
