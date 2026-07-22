using CrmTask.Application.Customers;
using CrmTask.Domain.Customers;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace CrmTask.Application.Tests.Customers;

public class UpdateCustomerRequestValidatorTests
{
    private readonly UpdateCustomerRequestValidator _sut = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var request = new UpdateCustomerRequest(
            "شرکت فناوران البرز",
            CustomerCategory.Legal,
            "02112345678",
            null,
            null,
            null,
            null,
            null,
            null,
            []);

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithMissingName_HasError(string name)
    {
        var request = new UpdateCustomerRequest(
            name,
            CustomerCategory.Legal,
            "02112345678",
            null,
            null,
            null,
            null,
            null,
            null,
            []);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Fact]
    public void Validate_WithPersonnelMissingFullName_HasError()
    {
        var request = new UpdateCustomerRequest(
            "شرکت فناوران البرز",
            CustomerCategory.Legal,
            "02112345678",
            null,
            null,
            null,
            null,
            null,
            null,
            [new CustomerPersonnelInput(string.Empty, null, null, null, null)]);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor("Personnel[0].FullName");
    }
}
