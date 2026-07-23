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

    [Fact]
    public void Create_SetsCreatedAtShamsiFromCreatedAt()
    {
        var customer = Customer.Create("شرکت فناوران البرز", CustomerCategory.Legal, "02112345678");

        customer.CreatedAtShamsi.Should().Be(CrmTask.Domain.Shared.PersianDateConverter.ToShamsi(customer.CreatedAt));
    }

    [Fact]
    public void UpdateCore_ChangesNameCategoryAndPhone()
    {
        var customer = Customer.Create("شرکت فناوران البرز", CustomerCategory.Legal, "02112345678");

        customer.UpdateCore("نام جدید", CustomerCategory.Individual, "09121234567");

        customer.Name.Should().Be("نام جدید");
        customer.Category.Should().Be(CustomerCategory.Individual);
        customer.Phone.Should().Be("09121234567");
    }

    [Fact]
    public void UpdateCore_WithMissingName_Throws()
    {
        var customer = Customer.Create("شرکت فناوران البرز", CustomerCategory.Legal, "02112345678");

        var act = () => customer.UpdateCore(string.Empty, CustomerCategory.Legal, "02112345678");

        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Fact]
    public void UpdateProfile_SetsOptionalFields()
    {
        var customer = Customer.Create("شرکت فناوران البرز", CustomerCategory.Legal, "02112345678");
        var managerBirthDate = new DateOnly(1980, 5, 10);

        customer.UpdateProfile(
            managerName: "رضا کیانی",
            managerBirthDate: managerBirthDate,
            address: "تهران، خیابان آزادی",
            fax: "02112345679",
            notes: "مشتری کلیدی",
            nationalId: "1234567890",
            categoryTitle: "صنعتی",
            activityField: "تولید قطعات خودرو");

        customer.ManagerName.Should().Be("رضا کیانی");
        customer.ManagerBirthDate.Should().Be(managerBirthDate);
        customer.ManagerBirthDateShamsi.Should().Be(CrmTask.Domain.Shared.PersianDateConverter.ToShamsi(managerBirthDate));
        customer.Address.Should().Be("تهران، خیابان آزادی");
        customer.Fax.Should().Be("02112345679");
        customer.Notes.Should().Be("مشتری کلیدی");
        customer.NationalId.Should().Be("1234567890");
        customer.CategoryTitle.Should().Be("صنعتی");
        customer.ActivityField.Should().Be("تولید قطعات خودرو");
    }

    [Fact]
    public void UpdateProfile_WithoutManagerBirthDate_LeavesShamsiMirrorNull()
    {
        var customer = Customer.Create("شرکت فناوران البرز", CustomerCategory.Legal, "02112345678");

        customer.UpdateProfile(null, null, null, null, null, null, null, null);

        customer.ManagerBirthDateShamsi.Should().BeNull();
    }
}
