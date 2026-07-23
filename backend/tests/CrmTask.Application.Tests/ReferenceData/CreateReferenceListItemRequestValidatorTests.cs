using CrmTask.Application.ReferenceData;
using FluentValidation.TestHelper;
using Xunit;

namespace CrmTask.Application.Tests.ReferenceData;

public class CreateReferenceListItemRequestValidatorTests
{
    private readonly CreateReferenceListItemRequestValidator _sut = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var request = new CreateReferenceListItemRequest("مسئول دفتر");

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithMissingTitle_HasError(string title)
    {
        var request = new CreateReferenceListItemRequest(title);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Title);
    }
}
