using CrmTask.Application.Settings;
using CrmTask.Domain.Settings;
using Microsoft.EntityFrameworkCore;

namespace CrmTask.Infrastructure.Settings;

public class SettingsRepository(CrmDbContext dbContext) : ISettingsRepository
{
    public async Task<AppSettings> GetAsync(CancellationToken cancellationToken = default)
    {
        var settings = await dbContext.AppSettings.FirstOrDefaultAsync(cancellationToken);
        if (settings is not null)
        {
            return settings;
        }

        settings = AppSettings.CreateDefault();
        await dbContext.AppSettings.AddAsync(settings, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return settings;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
