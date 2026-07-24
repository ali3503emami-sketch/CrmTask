namespace CrmTask.Domain.Settings;

/// <summary>
/// Global, admin-configurable system settings — a single row (see <see cref="SingletonId"/>),
/// not per-user, per product decision. Currently just the two "how many days ahead/before"
/// dashboard windows (تنظیمات > ابزار), but this is the one place any future global
/// numeric/toggle setting would live.
/// </summary>
public class AppSettings
{
    public static readonly Guid SingletonId = new("00000000-0000-0000-0000-000000000001");

    private AppSettings()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    /// <summary>
    /// Tasks due within this many days (or already overdue) show in the
    /// Dashboard's "کارهای جاری" tab — see docs/solution-structure.md.
    /// </summary>
    public int TaskUpcomingWindowDays { get; private set; }

    /// <summary>
    /// Passed to <see cref="Contracts.Contract.GetStatus"/> as the ExpiringSoon
    /// window — replaces what used to be a fixed 30-day constant.
    /// </summary>
    public int ContractEndingWindowDays { get; private set; }

    public static AppSettings CreateDefault() => new()
    {
        Id = SingletonId,
        TaskUpcomingWindowDays = 3,
        ContractEndingWindowDays = 30,
    };

    public void Update(int taskUpcomingWindowDays, int contractEndingWindowDays)
    {
        if (taskUpcomingWindowDays < 0)
        {
            throw new ArgumentException("Task upcoming window must not be negative.", nameof(taskUpcomingWindowDays));
        }

        if (contractEndingWindowDays < 0)
        {
            throw new ArgumentException("Contract ending window must not be negative.", nameof(contractEndingWindowDays));
        }

        TaskUpcomingWindowDays = taskUpcomingWindowDays;
        ContractEndingWindowDays = contractEndingWindowDays;
    }
}
