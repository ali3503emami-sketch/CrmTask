namespace CrmTask.Application.Settings;

public record UpdateAppSettingsRequest(int TaskUpcomingWindowDays, int ContractEndingWindowDays);
