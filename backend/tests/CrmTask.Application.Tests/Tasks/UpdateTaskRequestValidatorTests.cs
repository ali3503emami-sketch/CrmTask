using CrmTask.Application.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace CrmTask.Application.Tests.Tasks;

public class UpdateTaskRequestValidatorTests
{
    private static readonly DateTimeOffset DueAt = new(2026, 8, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid StaffId = Guid.NewGuid();
    private readonly UpdateTaskRequestValidator _sut = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var request = new UpdateTaskRequest("عنوان", string.Empty, DueAt, null, StaffId, StaffId, []);

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithMissingTitle_HasError(string title)
    {
        var request = new UpdateTaskRequest(title, string.Empty, DueAt, null, StaffId, StaffId, []);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Title);
    }

    [Fact]
    public void Validate_WithEmptyAssignedToStaffId_HasError()
    {
        var request = new UpdateTaskRequest("عنوان", string.Empty, DueAt, null, Guid.Empty, StaffId, []);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.AssignedToStaffId);
    }
}
