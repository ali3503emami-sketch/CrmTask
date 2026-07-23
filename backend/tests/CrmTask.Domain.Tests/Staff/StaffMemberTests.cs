using CrmTask.Domain.Staff;
using FluentAssertions;
using Xunit;

namespace CrmTask.Domain.Tests.Staff;

public class StaffMemberTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var staffMember = StaffMember.Create("سارا محمدی", "09121112233", "مسئول دفتر");

        staffMember.Id.Should().NotBeEmpty();
        staffMember.FullName.Should().Be("سارا محمدی");
        staffMember.PhoneNumber.Should().Be("09121112233");
        staffMember.Position.Should().Be("مسئول دفتر");
        staffMember.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithoutPosition_LeavesItNull()
    {
        var staffMember = StaffMember.Create("سارا محمدی", "09121112233", null);

        staffMember.Position.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithMissingFullName_Throws(string? fullName)
    {
        var act = () => StaffMember.Create(fullName!, "09121112233", null);

        act.Should().Throw<ArgumentException>().WithParameterName("fullName");
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        var staffMember = StaffMember.Create("سارا محمدی", "09121112233", null);

        staffMember.Deactivate();

        staffMember.IsActive.Should().BeFalse();
    }
}
