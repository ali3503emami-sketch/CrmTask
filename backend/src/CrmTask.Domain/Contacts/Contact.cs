using CrmTask.Domain.Shared;

namespace CrmTask.Domain.Contacts;

/// <summary>
/// A single logged call/contact with a customer, with an optional next follow-up date.
/// </summary>
public class Contact
{
    private Contact()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public ContactDirection Direction { get; private set; }

    public string Summary { get; private set; } = null!;

    public DateTimeOffset ContactedAt { get; private set; }

    public string ContactedAtShamsi { get; private set; } = null!;

    public DateTimeOffset? NextFollowUpAt { get; private set; }

    public string? NextFollowUpAtShamsi { get; private set; }

    public static Contact Create(
        Guid customerId,
        ContactDirection direction,
        string summary,
        DateTimeOffset contactedAt,
        DateTimeOffset? nextFollowUpAt)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("Customer id is required.", nameof(customerId));
        }

        if (string.IsNullOrWhiteSpace(summary))
        {
            throw new ArgumentException("Contact summary is required.", nameof(summary));
        }

        if (nextFollowUpAt.HasValue && nextFollowUpAt.Value <= contactedAt)
        {
            throw new ArgumentException("Next follow-up date must be after the contact date.", nameof(nextFollowUpAt));
        }

        return new Contact
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Direction = direction,
            Summary = summary.Trim(),
            ContactedAt = contactedAt,
            ContactedAtShamsi = PersianDateConverter.ToShamsi(contactedAt),
            NextFollowUpAt = nextFollowUpAt,
            NextFollowUpAtShamsi = nextFollowUpAt.HasValue ? PersianDateConverter.ToShamsi(nextFollowUpAt.Value) : null,
        };
    }
}
