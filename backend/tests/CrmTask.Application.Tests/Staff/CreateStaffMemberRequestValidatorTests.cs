using CrmTask.Application.Staff;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace CrmTask.Application.Tests.Staff;

public class CreateStaffMemberRequestValidatorTests
{
    private readonly CreateStaffMemberRequestValidator _sut = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var request = new CreateStaffMemberRequest("سارا محمدی", "09121112233", null);

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithMissingFullName_HasError(string fullName)
    {
        var request = new CreateStaffMemberRequest(fullName, "09121112233", null);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.FullName);
    }
}
