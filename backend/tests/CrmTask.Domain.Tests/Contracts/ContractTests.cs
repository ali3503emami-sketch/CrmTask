using CrmTask.Domain.Contracts;
using FluentAssertions;
using Xunit;

namespace CrmTask.Domain.Tests.Contracts;

public class ContractTests
{
    private static readonly Guid CustomerId = Guid.NewGuid();
    private static readonly DateOnly StartDate = new(2026, 1, 1);
    private static readonly DateOnly EndDate = new(2026, 12, 31);

    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var contract = Contract.Create(CustomerId, "قرارداد پشتیبانی سالانه", 50_000_000m, StartDate, EndDate);

        contract.Id.Should().NotBeEmpty();
        contract.CustomerId.Should().Be(CustomerId);
        contract.Title.Should().Be("قرارداد پشتیبانی سالانه");
        contract.Amount.Should().Be(50_000_000m);
        contract.StartDate.Should().Be(StartDate);
        contract.EndDate.Should().Be(EndDate);
    }

    [Fact]
    public void Create_WithEmptyCustomerId_Throws()
    {
        var act = () => Contract.Create(Guid.Empty, "عنوان", 0m, StartDate, EndDate);

        act.Should().Throw<ArgumentException>().WithParameterName("customerId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithMissingTitle_Throws(string? title)
    {
        var act = () => Contract.Create(CustomerId, title!, 0m, StartDate, EndDate);

        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Fact]
    public void Create_WithNegativeAmount_Throws()
    {
        var act = () => Contract.Create(CustomerId, "عنوان", -1m, StartDate, EndDate);

        act.Should().Throw<ArgumentException>().WithParameterName("amount");
    }

    [Fact]
    public void Create_WithEndDateAtOrBeforeStartDate_Throws()
    {
        var act = () => Contract.Create(CustomerId, "عنوان", 0m, StartDate, StartDate);

        act.Should().Throw<ArgumentException>().WithParameterName("endDate");
    }

    [Theory]
    [InlineData(2027, 1, 15, ContractStatus.Ended)]
    [InlineData(2026, 12, 15, ContractStatus.ExpiringSoon)]
    [InlineData(2026, 3, 1, ContractStatus.Active)]
    public void GetStatus_ClassifiesBasedOnEndDateAndToday(int year, int month, int day, ContractStatus expected)
    {
        var today = new DateOnly(year, month, day);
        var contract = Contract.Create(CustomerId, "عنوان", 0m, StartDate, EndDate);

        contract.GetStatus(today).Should().Be(expected);
    }

    [Fact]
    public void Create_SetsShamsiMirrorsForBothDates()
    {
        var contract = Contract.Create(CustomerId, "عنوان", 0m, StartDate, EndDate);

        contract.StartDateShamsi.Should().Be(CrmTask.Domain.Shared.PersianDateConverter.ToShamsi(StartDate));
        contract.EndDateShamsi.Should().Be(CrmTask.Domain.Shared.PersianDateConverter.ToShamsi(EndDate));
    }
}
