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

    [Fact]
    public async Task UpdateAsync_WithValidRequest_UpdatesCoreAndProfileAndPersonnel()
    {
        var customer = Customer.Create("نام قدیمی", CustomerCategory.Legal, "02112345678");
        _repository.Setup(r => r.GetTrackedByIdAsync(customer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        var request = new UpdateCustomerRequest(
            "نام جدید",
            CustomerCategory.Individual,
            "09121234567",
            "رضا کیانی",
            new DateOnly(1980, 5, 10),
            "تهران",
            "02112345679",
            "یادداشت",
            "1234567890",
            "صنعتی",
            "تولید قطعات خودرو",
            [new CustomerPersonnelInput("سارا محمدی", "حسابدار", null, "09121112233", null)]);

        var result = await _sut.UpdateAsync(customer.Id, request);

        result!.Name.Should().Be("نام جدید");
        result.ManagerName.Should().Be("رضا کیانی");
        result.CategoryTitle.Should().Be("صنعتی");
        result.ActivityField.Should().Be("تولید قطعات خودرو");
        result.Personnel.Should().ContainSingle(p => p.FullName == "سارا محمدی");
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerNotFound_ReturnsNull()
    {
        _repository.Setup(r => r.GetTrackedByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);
        var request = new UpdateCustomerRequest("نام", CustomerCategory.Legal, "02112345678", null, null, null, null, null, null, null, null, []);

        var result = await _sut.UpdateAsync(Guid.NewGuid(), request);

        result.Should().BeNull();
    }
}
