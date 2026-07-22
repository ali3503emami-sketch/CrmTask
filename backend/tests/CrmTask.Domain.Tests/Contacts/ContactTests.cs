using CrmTask.Domain.Contacts;
using FluentAssertions;
using Xunit;

namespace CrmTask.Domain.Tests.Contacts;

public class ContactTests
{
    private static readonly Guid CustomerId = Guid.NewGuid();
    private static readonly DateTimeOffset ContactedAt = new(2026, 7, 20, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var followUp = ContactedAt.AddDays(3);

        var contact = Contact.Create(CustomerId, ContactDirection.Outbound, "پیگیری وضعیت قرارداد", ContactedAt, followUp);

        contact.Id.Should().NotBeEmpty();
        contact.CustomerId.Should().Be(CustomerId);
        contact.Direction.Should().Be(ContactDirection.Outbound);
        contact.Summary.Should().Be("پیگیری وضعیت قرارداد");
        contact.ContactedAt.Should().Be(ContactedAt);
        contact.NextFollowUpAt.Should().Be(followUp);
    }

    [Fact]
    public void Create_WithoutFollowUp_LeavesItNull()
    {
        var contact = Contact.Create(CustomerId, ContactDirection.Inbound, "تماس ورودی مشتری", ContactedAt, nextFollowUpAt: null);

        contact.NextFollowUpAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyCustomerId_Throws()
    {
        var act = () => Contact.Create(Guid.Empty, ContactDirection.Inbound, "خلاصه", ContactedAt, null);

        act.Should().Throw<ArgumentException>().WithParameterName("customerId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithMissingSummary_Throws(string? summary)
    {
        var act = () => Contact.Create(CustomerId, ContactDirection.Inbound, summary!, ContactedAt, null);

        act.Should().Throw<ArgumentException>().WithParameterName("summary");
    }

    [Fact]
    public void Create_WithFollowUpAtOrBeforeContactDate_Throws()
    {
        var act = () => Contact.Create(CustomerId, ContactDirection.Inbound, "خلاصه", ContactedAt, ContactedAt);

        act.Should().Throw<ArgumentException>().WithParameterName("nextFollowUpAt");
    }

    [Fact]
    public void Create_SetsShamsiMirrorsForBothDates()
    {
        var followUp = ContactedAt.AddDays(3);

        var contact = Contact.Create(CustomerId, ContactDirection.Outbound, "خلاصه", ContactedAt, followUp);

        contact.ContactedAtShamsi.Should().Be(CrmTask.Domain.Shared.PersianDateConverter.ToShamsi(ContactedAt));
        contact.NextFollowUpAtShamsi.Should().Be(CrmTask.Domain.Shared.PersianDateConverter.ToShamsi(followUp));
    }

    [Fact]
    public void Create_WithoutFollowUp_LeavesShamsiMirrorNull()
    {
        var contact = Contact.Create(CustomerId, ContactDirection.Inbound, "خلاصه", ContactedAt, null);

        contact.NextFollowUpAtShamsi.Should().BeNull();
    }
}
