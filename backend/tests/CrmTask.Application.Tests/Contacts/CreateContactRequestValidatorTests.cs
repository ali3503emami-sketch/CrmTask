using CrmTask.Application.Contacts;
using CrmTask.Domain.Contacts;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace CrmTask.Application.Tests.Contacts;

public class CreateContactRequestValidatorTests
{
    private static readonly DateTimeOffset ContactedAt = new(2026, 7, 20, 10, 0, 0, TimeSpan.Zero);
    private readonly CreateContactRequestValidator _sut = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var request = new CreateContactRequest(ContactDirection.Outbound, "پیگیری وضعیت قرارداد", ContactedAt, ContactedAt.AddDays(3));

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithMissingSummary_HasError(string summary)
    {
        var request = new CreateContactRequest(ContactDirection.Inbound, summary, ContactedAt, null);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Summary);
    }

    [Fact]
    public void Validate_WithFollowUpBeforeContactDate_HasError()
    {
        var request = new CreateContactRequest(ContactDirection.Inbound, "خلاصه", ContactedAt, ContactedAt.AddDays(-1));

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.NextFollowUpAt);
    }
}
