using CrmTask.Application.Contracts;
using CrmTask.Application.Tests.TestSupport;
using CrmTask.Domain.Contracts;
using FluentAssertions;
using Moq;
using Xunit;

namespace CrmTask.Application.Tests.Contracts;

public class ContractServiceTests
{
    private static readonly Guid CustomerId = Guid.NewGuid();
    private static readonly DateOnly StartDate = new(2026, 1, 1);
    private static readonly DateOnly EndDate = new(2026, 12, 31);

    private readonly Mock<IContractRepository> _repository = new();
    private readonly FakeTimeProvider _timeProvider = new(new DateTimeOffset(2026, 12, 20, 0, 0, 0, TimeSpan.Zero));
    private readonly ContractService _sut;

    public ContractServiceTests()
    {
        _sut = new ContractService(_repository.Object, _timeProvider);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_SavesAndReturnsContract()
    {
        var request = new CreateContractRequest("قرارداد پشتیبانی سالانه", 50_000_000m, StartDate, EndDate);

        var result = await _sut.CreateAsync(CustomerId, request);

        result.Title.Should().Be("قرارداد پشتیبانی سالانه");
        result.CustomerId.Should().Be(CustomerId);
        _repository.Verify(r => r.AddAsync(It.IsAny<Contract>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_IncludesStatusComputedAgainstTheInjectedClock_NotWallClock()
    {
        // The fake clock is fixed at 2026-12-20, 11 days before the 2026-12-31
        // end date — inside the 30-day expiring-soon window, regardless of
        // what day this test actually runs on.
        var request = new CreateContractRequest("قرارداد", 0m, StartDate, EndDate);

        var result = await _sut.CreateAsync(CustomerId, request);

        result.Status.Should().Be(ContractStatus.ExpiringSoon);
    }

    [Fact]
    public async Task GetByCustomerIdAsync_ReturnsContractsForThatCustomer()
    {
        var contracts = new[]
        {
            Contract.Create(CustomerId, "قرارداد اول", 0m, StartDate, EndDate),
            Contract.Create(CustomerId, "قرارداد دوم", 0m, StartDate, EndDate),
        };
        _repository.Setup(r => r.GetByCustomerIdAsync(CustomerId, It.IsAny<CancellationToken>())).ReturnsAsync(contracts);

        var result = await _sut.GetByCustomerIdAsync(CustomerId);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsContractsAcrossAllCustomers()
    {
        var otherCustomerId = Guid.NewGuid();
        var contracts = new[]
        {
            Contract.Create(CustomerId, "قرارداد اول", 0m, StartDate, EndDate),
            Contract.Create(otherCustomerId, "قرارداد دوم", 0m, StartDate, EndDate),
        };
        _repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(contracts);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }
}
