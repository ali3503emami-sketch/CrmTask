using CrmTask.Application.Contracts;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace CrmTask.Application.Tests.Contracts;

public class CreateContractRequestValidatorTests
{
    private static readonly DateOnly StartDate = new(2026, 1, 1);
    private static readonly DateOnly EndDate = new(2026, 12, 31);
    private readonly CreateContractRequestValidator _sut = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var request = new CreateContractRequest("قرارداد پشتیبانی سالانه", 50_000_000m, StartDate, EndDate);

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithMissingTitle_HasError(string title)
    {
        var request = new CreateContractRequest(title, 0m, StartDate, EndDate);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Title);
    }

    [Fact]
    public void Validate_WithNegativeAmount_HasError()
    {
        var request = new CreateContractRequest("عنوان", -1m, StartDate, EndDate);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Amount);
    }

    [Fact]
    public void Validate_WithEndDateAtOrBeforeStartDate_HasError()
    {
        var request = new CreateContractRequest("عنوان", 0m, StartDate, StartDate);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.EndDate);
    }
}
