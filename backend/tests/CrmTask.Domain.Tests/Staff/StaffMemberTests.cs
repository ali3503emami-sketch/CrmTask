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

    [Fact]
    public void Update_WithValidData_ChangesProperties()
    {
        var staffMember = StaffMember.Create("سارا محمدی", "09121112233", null);

        staffMember.Update("سارا احمدی", "09129998877", "مدیر دفتر");

        staffMember.FullName.Should().Be("سارا احمدی");
        staffMember.PhoneNumber.Should().Be("09129998877");
        staffMember.Position.Should().Be("مدیر دفتر");
    }

    [Fact]
    public void Update_WithoutPosition_ClearsIt()
    {
        var staffMember = StaffMember.Create("سارا محمدی", "09121112233", "مسئول دفتر");

        staffMember.Update("سارا محمدی", "09121112233", null);

        staffMember.Position.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Update_WithMissingFullName_Throws(string? fullName)
    {
        var staffMember = StaffMember.Create("سارا محمدی", "09121112233", null);

        var act = () => staffMember.Update(fullName!, "09121112233", null);

        act.Should().Throw<ArgumentException>().WithParameterName("fullName");
    }
}
