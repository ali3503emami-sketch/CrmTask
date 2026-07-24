namespace CrmTask.Application.Settings;

public interface ISettingsService
{
    Task<AppSettingsDto> GetAsync(CancellationToken cancellationToken = default);

    Task<AppSettingsDto> UpdateAsync(UpdateAppSettingsRequest request, CancellationToken cancellationToken = default);
}
