namespace CrmTask.Domain.Staff;

/// <summary>
/// An internal staff member — the pool of people work items can be referred/assigned to.
/// </summary>
public class StaffMember
{
    private StaffMember()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    public string FullName { get; private set; } = null!;

    public string PhoneNumber { get; private set; } = null!;

    public string? Position { get; private set; }

    public bool IsActive { get; private set; }

    public static StaffMember Create(string fullName, string phoneNumber, string? position)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Full name is required.", nameof(fullName));
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException("Phone number is required.", nameof(phoneNumber));
        }

        return new StaffMember
        {
            Id = Guid.NewGuid(),
            FullName = fullName.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            Position = string.IsNullOrWhiteSpace(position) ? null : position.Trim(),
            IsActive = true,
        };
    }

    public void Deactivate() => IsActive = false;

    public void Update(string fullName, string phoneNumber, string? position)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Full name is required.", nameof(fullName));
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException("Phone number is required.", nameof(phoneNumber));
        }

        FullName = fullName.Trim();
        PhoneNumber = phoneNumber.Trim();
        Position = string.IsNullOrWhiteSpace(position) ? null : position.Trim();
    }
}
