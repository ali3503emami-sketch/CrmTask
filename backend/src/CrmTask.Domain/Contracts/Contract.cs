using CrmTask.Domain.Shared;

namespace CrmTask.Domain.Contracts;

/// <summary>
/// A contract with a customer, running from <see cref="StartDate"/> to <see cref="EndDate"/>.
/// </summary>
public class Contract
{
    private Contract()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public string Title { get; private set; } = null!;

    public decimal Amount { get; private set; }

    public DateOnly StartDate { get; private set; }

    public string StartDateShamsi { get; private set; } = null!;

    public DateOnly EndDate { get; private set; }

    public string EndDateShamsi { get; private set; } = null!;

    public static Contract Create(Guid customerId, string title, decimal amount, DateOnly startDate, DateOnly endDate)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("Customer id is required.", nameof(customerId));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Contract title is required.", nameof(title));
        }

        if (amount < 0)
        {
            throw new ArgumentException("Contract amount cannot be negative.", nameof(amount));
        }

        if (endDate <= startDate)
        {
            throw new ArgumentException("End date must be after the start date.", nameof(endDate));
        }

        return new Contract
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Title = title.Trim(),
            Amount = amount,
            StartDate = startDate,
            StartDateShamsi = PersianDateConverter.ToShamsi(startDate),
            EndDate = endDate,
            EndDateShamsi = PersianDateConverter.ToShamsi(endDate),
        };
    }

    /// <param name="today">The current date, per the injected <see cref="TimeProvider"/>.</param>
    /// <param name="expiringSoonWindowDays">
    /// Within this many days of <see cref="EndDate"/>, the contract counts as expiring
    /// soon — a global, admin-configurable value (see <c>AppSettings.ContractEndingWindowDays</c>),
    /// not a fixed constant, so callers must source it from Settings rather than hardcode it.
    /// </param>
    public ContractStatus GetStatus(DateOnly today, int expiringSoonWindowDays)
    {
        if (EndDate < today)
        {
            return ContractStatus.Ended;
        }

        return EndDate <= today.AddDays(expiringSoonWindowDays) ? ContractStatus.ExpiringSoon : ContractStatus.Active;
    }
}
