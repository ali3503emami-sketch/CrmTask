using CrmTask.Application.Customers;
using CrmTask.Domain.Customers;
using FluentAssertions;
using Moq;
using Xunit;

namespace CrmTask.Application.Tests.Customers;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _repository = new();
    private readonly CustomerService _sut;

    public CustomerServiceTests()
    {
        _sut = new CustomerService(_repository.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_SavesAndReturnsCustomer()
    {
        var request = new CreateCustomerRequest("شرکت فناوران البرز", CustomerCategory.Legal, "02112345678");

        var result = await _sut.CreateAsync(request);

        result.Name.Should().Be("شرکت فناوران البرز");
        result.Category.Should().Be(CustomerCategory.Legal);
        result.Phone.Should().Be("02112345678");
        _repository.Verify(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCustomersFromRepository()
    {
        var customers = new[]
        {
            Customer.Create("شرکت فناوران البرز", CustomerCategory.Legal, "02112345678"),
            Customer.Create("مهندس رضا کیانی", CustomerCategory.Individual, "09121234567"),
        };
        _repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(customers);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
        result.Select(c => c.Name).Should().Contain(["شرکت فناوران البرز", "مهندس رضا کیانی"]);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }
}
