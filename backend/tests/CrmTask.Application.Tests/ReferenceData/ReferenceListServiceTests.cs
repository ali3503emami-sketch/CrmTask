using CrmTask.Application.ReferenceData;
using CrmTask.Domain.ReferenceData;
using FluentAssertions;
using Moq;
using Xunit;

namespace CrmTask.Application.Tests.ReferenceData;

public class ReferenceListServiceTests
{
    private readonly Mock<IReferenceListRepository> _repository = new();
    private readonly ReferenceListService _sut;

    public ReferenceListServiceTests()
    {
        _sut = new ReferenceListService(_repository.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_SavesAndReturnsItem()
    {
        var request = new CreateReferenceListItemRequest("مسئول دفتر");

        var result = await _sut.CreateAsync(ReferenceListKind.Position, request);

        result.Title.Should().Be("مسئول دفتر");
        _repository.Verify(r => r.AddAsync(It.IsAny<ReferenceListItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByKindAsync_ReturnsItemsFromRepositoryForThatKind()
    {
        var item = ReferenceListItem.Create(ReferenceListKind.CustomerCategory, "صنعتی");
        _repository.Setup(r => r.GetByKindAsync(ReferenceListKind.CustomerCategory, It.IsAny<CancellationToken>()))
            .ReturnsAsync([item]);

        var result = await _sut.GetByKindAsync(ReferenceListKind.CustomerCategory);

        result.Should().ContainSingle(i => i.Title == "صنعتی");
    }

    [Fact]
    public async Task UpdateAsync_WhenFound_UpdatesTitleAndSaves()
    {
        var item = ReferenceListItem.Create(ReferenceListKind.Position, "مسئول دفتر");
        _repository.Setup(r => r.GetTrackedByIdAsync(item.Id, It.IsAny<CancellationToken>())).ReturnsAsync(item);

        var result = await _sut.UpdateAsync(item.Id, new CreateReferenceListItemRequest("مدیر دفتر"));

        result!.Title.Should().Be("مدیر دفتر");
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotFound_ReturnsNull()
    {
        _repository.Setup(r => r.GetTrackedByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReferenceListItem?)null);

        var result = await _sut.UpdateAsync(Guid.NewGuid(), new CreateReferenceListItemRequest("مدیر دفتر"));

        result.Should().BeNull();
    }
}
