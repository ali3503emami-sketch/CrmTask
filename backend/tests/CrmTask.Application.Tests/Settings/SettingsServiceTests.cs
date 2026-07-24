using CrmTask.Application.Settings;
using CrmTask.Domain.Settings;
using FluentAssertions;
using Moq;
using Xunit;

namespace CrmTask.Application.Tests.Settings;

public class SettingsServiceTests
{
    private readonly Mock<ISettingsRepository> _repository = new();
    private readonly SettingsService _sut;

    public SettingsServiceTests()
    {
        _sut = new SettingsService(_repository.Object);
    }

    [Fact]
    public async Task GetAsync_ReturnsCurrentSettings()
    {
        var settings = AppSettings.CreateDefault();
        _repository.Setup(r => r.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(settings);

        var result = await _sut.GetAsync();

        result.TaskUpcomingWindowDays.Should().Be(3);
        result.ContractEndingWindowDays.Should().Be(30);
    }

    [Fact]
    public async Task UpdateAsync_ChangesValuesAndSaves()
    {
        var settings = AppSettings.CreateDefault();
        _repository.Setup(r => r.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(settings);
        var request = new UpdateAppSettingsRequest(5, 45);

        var result = await _sut.UpdateAsync(request);

        result.TaskUpcomingWindowDays.Should().Be(5);
        result.ContractEndingWindowDays.Should().Be(45);
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
