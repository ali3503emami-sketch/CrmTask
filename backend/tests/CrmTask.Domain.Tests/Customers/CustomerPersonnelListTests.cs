using CrmTask.Domain.Customers;
using FluentAssertions;
using Xunit;

namespace CrmTask.Domain.Tests.Customers;

public class CustomerPersonnelListTests
{
    [Fact]
    public void ReplacePersonnel_SetsThePersonnelList()
    {
        var customer = Customer.Create("شرکت فناوران البرز", CustomerCategory.Legal, "02112345678");
        var personnel = new[]
        {
            CustomerPersonnel.Create("رضا کیانی", "مدیر فروش", "02112345678", null, null),
            CustomerPersonnel.Create("سارا محمدی", "حسابدار", null, "09121112233", null),
        };

        customer.ReplacePersonnel(personnel);

        customer.Personnel.Should().HaveCount(2);
    }

    [Fact]
    public void ReplacePersonnel_CalledAgain_ReplacesThePreviousList()
    {
        var customer = Customer.Create("شرکت فناوران البرز", CustomerCategory.Legal, "02112345678");
        customer.ReplacePersonnel([CustomerPersonnel.Create("رضا کیانی", null, null, null, null)]);

        customer.ReplacePersonnel([CustomerPersonnel.Create("سارا محمدی", null, null, null, null)]);

        customer.Personnel.Should().ContainSingle(p => p.FullName == "سارا محمدی");
    }
}
