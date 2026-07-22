using CrmTask.Domain.Shared;
using FluentAssertions;
using Xunit;

namespace CrmTask.Domain.Tests.Shared;

public class PersianDateConverterTests
{
    [Fact]
    public void ToShamsi_DateOnly_ConvertsKnownHistoricalDate()
    {
        // 22 Bahman 1357 is the well-documented Persian-calendar date of the
        // 1979 revolution's victory day — a fixed, independently verifiable
        // conversion oracle, not something computed by this same code.
        var date = new DateOnly(1979, 2, 11);

        PersianDateConverter.ToShamsi(date).Should().Be("1357/11/22");
    }

    [Fact]
    public void ToShamsi_DateOnly_ConvertsNowruz1403()
    {
        // Nowruz (Persian new year) 1403 fell on 2024-03-20 — also a fixed,
        // publicly documented calendar fact.
        var date = new DateOnly(2024, 3, 20);

        PersianDateConverter.ToShamsi(date).Should().Be("1403/01/01");
    }

    [Fact]
    public void ToShamsi_DateTimeOffset_UsesTheDatePortionOnly()
    {
        var date = new DateTimeOffset(2024, 3, 20, 18, 30, 0, TimeSpan.Zero);

        PersianDateConverter.ToShamsi(date).Should().Be("1403/01/01");
    }

    [Fact]
    public void ToShamsi_PadsMonthAndDayToTwoDigits()
    {
        var date = new DateOnly(2024, 3, 21);

        PersianDateConverter.ToShamsi(date).Should().Be("1403/01/02");
    }
}
