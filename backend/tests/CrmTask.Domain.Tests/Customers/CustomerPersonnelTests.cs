using CrmTask.Domain.Customers;
using FluentAssertions;
using Xunit;

namespace CrmTask.Domain.Tests.Customers;

public class CustomerPersonnelTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var person = CustomerPersonnel.Create("رضا کیانی", "مدیر فروش", "02112345678", "09121234567", "reza@example.com");

        person.Id.Should().NotBeEmpty();
        person.FullName.Should().Be("رضا کیانی");
        person.Position.Should().Be("مدیر فروش");
        person.Phone.Should().Be("02112345678");
        person.Mobile.Should().Be("09121234567");
        person.Email.Should().Be("reza@example.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithMissingFullName_Throws(string? fullName)
    {
        var act = () => CustomerPersonnel.Create(fullName!, null, null, null, null);

        act.Should().Throw<ArgumentException>().WithParameterName("fullName");
    }

    [Fact]
    public void Create_WithOnlyFullName_LeavesOthersNull()
    {
        var person = CustomerPersonnel.Create("رضا کیانی", null, null, null, null);

        person.Position.Should().BeNull();
        person.Phone.Should().BeNull();
        person.Mobile.Should().BeNull();
        person.Email.Should().BeNull();
    }
}
