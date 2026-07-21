using CrmTask.Domain.Customers;
using FluentAssertions;
using Xunit;

namespace CrmTask.Domain.Tests.Customers;

public class CustomerTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var customer = Customer.Create("شرکت فناوران البرز", CustomerCategory.Legal, "02112345678");

        customer.Id.Should().NotBeEmpty();
        customer.Name.Should().Be("شرکت فناوران البرز");
        customer.Category.Should().Be(CustomerCategory.Legal);
        customer.Phone.Should().Be("02112345678");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithMissingName_Throws(string? name)
    {
        var act = () => Customer.Create(name!, CustomerCategory.Individual, "02112345678");

        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithMissingPhone_Throws(string? phone)
    {
        var act = () => Customer.Create("مهندس رضا کیانی", CustomerCategory.Individual, phone!);

        act.Should().Throw<ArgumentException>().WithParameterName("phone");
    }

    [Fact]
    public void Create_TrimsNameAndPhone()
    {
        var customer = Customer.Create("  گروه صنعتی پارسیان  ", CustomerCategory.Legal, "  02112345678  ");

        customer.Name.Should().Be("گروه صنعتی پارسیان");
        customer.Phone.Should().Be("02112345678");
    }
}
