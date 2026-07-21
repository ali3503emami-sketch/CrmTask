using CrmTask.Application.Customers;
using CrmTask.Domain.Customers;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace CrmTask.Application.Tests.Customers;

public class CreateCustomerRequestValidatorTests
{
    private readonly CreateCustomerRequestValidator _sut = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var request = new CreateCustomerRequest("شرکت فناوران البرز", CustomerCategory.Legal, "02112345678");

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithMissingName_HasError(string name)
    {
        var request = new CreateCustomerRequest(name, CustomerCategory.Legal, "02112345678");

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-phone")]
    public void Validate_WithInvalidPhone_HasError(string phone)
    {
        var request = new CreateCustomerRequest("شرکت فناوران البرز", CustomerCategory.Legal, phone);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Phone);
    }
}
