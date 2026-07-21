using CrmTask.Domain.Staff;
using FluentAssertions;
using Xunit;

namespace CrmTask.Domain.Tests.Staff;

public class StaffMemberTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var staffMember = StaffMember.Create("سارا محمدی", "09121112233");

        staffMember.Id.Should().NotBeEmpty();
        staffMember.FullName.Should().Be("سارا محمدی");
        staffMember.PhoneNumber.Should().Be("09121112233");
        staffMember.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithMissingFullName_Throws(string? fullName)
    {
        var act = () => StaffMember.Create(fullName!, "09121112233");

        act.Should().Throw<ArgumentException>().WithParameterName("fullName");
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        var staffMember = StaffMember.Create("سارا محمدی", "09121112233");

        staffMember.Deactivate();

        staffMember.IsActive.Should().BeFalse();
    }
}
