namespace CrmTask.Application.Settings;

public class SettingsService(ISettingsRepository repository) : ISettingsService
{
    public async Task<AppSettingsDto> GetAsync(CancellationToken cancellationToken = default)
    {
        var settings = await repository.GetAsync(cancellationToken);

        return ToDto(settings);
    }

    public async Task<AppSettingsDto> UpdateAsync(UpdateAppSettingsRequest request, CancellationToken cancellationToken = default)
    {
        var settings = await repository.GetAsync(cancellationToken);

        settings.Update(request.TaskUpcomingWindowDays, request.ContractEndingWindowDays);
        await repository.SaveChangesAsync(cancellationToken);

        return ToDto(settings);
    }

    private static AppSettingsDto ToDto(Domain.Settings.AppSettings settings) =>
        new(settings.TaskUpcomingWindowDays, settings.ContractEndingWindowDays);
}
