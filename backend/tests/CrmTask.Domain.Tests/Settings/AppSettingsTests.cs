using CrmTask.Domain.Settings;
using FluentAssertions;
using Xunit;

namespace CrmTask.Domain.Tests.Settings;

public class AppSettingsTests
{
    [Fact]
    public void CreateDefault_SetsSensibleDefaults()
    {
        var settings = AppSettings.CreateDefault();

        settings.Id.Should().Be(AppSettings.SingletonId);
        settings.TaskUpcomingWindowDays.Should().Be(3);
        settings.ContractEndingWindowDays.Should().Be(30);
    }

    [Fact]
    public void Update_WithValidValues_ChangesThem()
    {
        var settings = AppSettings.CreateDefault();

        settings.Update(5, 45);

        settings.TaskUpcomingWindowDays.Should().Be(5);
        settings.ContractEndingWindowDays.Should().Be(45);
    }

    [Fact]
    public void Update_WithNegativeTaskWindow_Throws()
    {
        var settings = AppSettings.CreateDefault();

        var act = () => settings.Update(-1, 30);

        act.Should().Throw<ArgumentException>().WithParameterName("taskUpcomingWindowDays");
    }

    [Fact]
    public void Update_WithNegativeContractWindow_Throws()
    {
        var settings = AppSettings.CreateDefault();

        var act = () => settings.Update(3, -1);

        act.Should().Throw<ArgumentException>().WithParameterName("contractEndingWindowDays");
    }
}
