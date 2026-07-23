using CrmTask.Application.Staff;
using CrmTask.Domain.Staff;
using FluentAssertions;
using Moq;
using Xunit;

namespace CrmTask.Application.Tests.Staff;

public class StaffServiceTests
{
    private readonly Mock<IStaffRepository> _repository = new();
    private readonly StaffService _sut;

    public StaffServiceTests()
    {
        _sut = new StaffService(_repository.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_SavesAndReturnsStaffMember()
    {
        var request = new CreateStaffMemberRequest("سارا محمدی", "09121112233", "مسئول دفتر");

        var result = await _sut.CreateAsync(request);

        result.FullName.Should().Be("سارا محمدی");
        _repository.Verify(r => r.AddAsync(It.IsAny<StaffMember>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActiveStaffFromRepository()
    {
        var active = StaffMember.Create("سارا محمدی", "09121112233", null);
        _repository.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync([active]);

        var result = await _sut.GetActiveAsync();

        result.Should().ContainSingle(s => s.FullName == "سارا محمدی");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StaffMember?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }
}
