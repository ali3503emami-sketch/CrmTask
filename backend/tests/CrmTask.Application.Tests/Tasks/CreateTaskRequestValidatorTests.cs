using CrmTask.Application.Tasks;
using CrmTask.Domain.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace CrmTask.Application.Tests.Tasks;

public class CreateTaskRequestValidatorTests
{
    private static readonly DateTimeOffset DueAt = new(2026, 8, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid StaffId = Guid.NewGuid();
    private readonly CreateTaskRequestValidator _sut = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var request = new CreateTaskRequest(
            "بررسی سرور",
            "توضیحات",
            DueAt,
            null,
            StaffId,
            [new ChecklistFieldDefinition("چک شد؟", ChecklistFieldType.Checkbox, null)]);

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithMissingTitle_HasError(string title)
    {
        var request = new CreateTaskRequest(title, string.Empty, DueAt, null, StaffId, []);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Title);
    }

    [Fact]
    public void Validate_WithEmptyAssignedStaffId_HasError()
    {
        var request = new CreateTaskRequest("عنوان", string.Empty, DueAt, null, Guid.Empty, []);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.AssignedToStaffId);
    }

    [Fact]
    public void Validate_WithDropdownFieldMissingOptions_HasError()
    {
        var request = new CreateTaskRequest(
            "عنوان",
            string.Empty,
            DueAt,
            null,
            StaffId,
            [new ChecklistFieldDefinition("وضعیت", ChecklistFieldType.Dropdown, null)]);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor("ChecklistFields[0].Options");
    }
}
