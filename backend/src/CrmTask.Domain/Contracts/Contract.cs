namespace CrmTask.Domain.Contracts;

/// <summary>
/// A contract with a customer, running from <see cref="StartDate"/> to <see cref="EndDate"/>.
/// </summary>
public class Contract
{
    /// <summary>
    /// Within this many days of <see cref="EndDate"/>, a contract counts as expiring soon.
    /// </summary>
    private const int ExpiringSoonWindowDays = 30;

    private Contract()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public string Title { get; private set; } = null!;

    public decimal Amount { get; private set; }

    public DateOnly StartDate { get; private set; }

    public DateOnly EndDate { get; private set; }

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
            EndDate = endDate,
        };
    }

    public ContractStatus GetStatus(DateOnly today)
    {
        if (EndDate < today)
        {
            return ContractStatus.Ended;
        }

        return EndDate <= today.AddDays(ExpiringSoonWindowDays) ? ContractStatus.ExpiringSoon : ContractStatus.Active;
    }
}
