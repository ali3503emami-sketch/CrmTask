using CrmTask.Domain.Settings;

namespace CrmTask.Application.Settings;

public interface ISettingsRepository
{
    /// <summary>
    /// Returns the single settings row, creating and persisting the default
    /// one on first access — there's no separate seeding step, Settings just
    /// always resolves to *something* usable.
    /// </summary>
    Task<AppSettings> GetAsync(CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
