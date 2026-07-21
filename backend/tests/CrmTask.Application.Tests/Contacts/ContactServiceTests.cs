using CrmTask.Application.Contacts;
using CrmTask.Domain.Contacts;
using FluentAssertions;
using Moq;
using Xunit;

namespace CrmTask.Application.Tests.Contacts;

public class ContactServiceTests
{
    private static readonly Guid CustomerId = Guid.NewGuid();
    private static readonly DateTimeOffset ContactedAt = new(2026, 7, 20, 10, 0, 0, TimeSpan.Zero);

    private readonly Mock<IContactRepository> _repository = new();
    private readonly ContactService _sut;

    public ContactServiceTests()
    {
        _sut = new ContactService(_repository.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_SavesAndReturnsContact()
    {
        var request = new CreateContactRequest(ContactDirection.Outbound, "پیگیری وضعیت قرارداد", ContactedAt, ContactedAt.AddDays(3));

        var result = await _sut.CreateAsync(CustomerId, request);

        result.Summary.Should().Be("پیگیری وضعیت قرارداد");
        result.CustomerId.Should().Be(CustomerId);
        _repository.Verify(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByCustomerIdAsync_ReturnsContactsForThatCustomer()
    {
        var contacts = new[]
        {
            Contact.Create(CustomerId, ContactDirection.Inbound, "تماس اول", ContactedAt, null),
            Contact.Create(CustomerId, ContactDirection.Outbound, "تماس دوم", ContactedAt.AddDays(1), null),
        };
        _repository.Setup(r => r.GetByCustomerIdAsync(CustomerId, It.IsAny<CancellationToken>())).ReturnsAsync(contacts);

        var result = await _sut.GetByCustomerIdAsync(CustomerId);

        result.Should().HaveCount(2);
    }
}
