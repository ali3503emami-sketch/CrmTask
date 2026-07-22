using CrmTask.Application.Tasks;
using CrmTask.Domain.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace CrmTask.Application.Tests.Tasks;

public class TaskServiceTests
{
    private static readonly DateTimeOffset DueAt = new(2026, 8, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid StaffId = Guid.NewGuid();

    private readonly Mock<ITaskRepository> _repository = new();
    private readonly TaskService _sut;

    public TaskServiceTests()
    {
        _sut = new TaskService(_repository.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_SavesAndReturnsTask()
    {
        var request = new CreateTaskRequest(
            "بررسی سرور",
            "توضیحات",
            DueAt,
            CustomerId: null,
            StaffId,
            ChecklistFields: [new ChecklistFieldDefinition("چک شد؟", ChecklistFieldType.Checkbox, null)]);

        var result = await _sut.CreateAsync(request);

        result.Title.Should().Be("بررسی سرور");
        result.ChecklistItems.Should().ContainSingle(i => i.Label == "چک شد؟");
        _repository.Verify(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsDoneAsync_UpdatesStatusAndPersists()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, []);
        _repository.Setup(r => r.GetByIdAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);

        var result = await _sut.MarkAsDoneAsync(task.Id);

        result!.Status.Should().Be(TaskItemStatus.Done);
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsDoneAsync_WhenTaskNotFound_ReturnsNull()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((TaskItem?)null);

        var result = await _sut.MarkAsDoneAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task SetChecklistItemValueAsync_UpdatesTheItemAndPersists()
    {
        var checklistItem = ChecklistItem.Create("چک شد؟", ChecklistFieldType.Checkbox, null);
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, [checklistItem]);
        _repository.Setup(r => r.GetByIdAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);

        var result = await _sut.SetChecklistItemValueAsync(task.Id, checklistItem.Id, "true");

        result!.ChecklistItems.Single().Value.Should().Be("true");
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsTasksFromRepository()
    {
        var tasks = new[] { TaskItem.Create("کار اول", string.Empty, DueAt, null, StaffId, []) };
        _repository.Setup(r => r.GetAllAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(tasks);

        var result = await _sut.GetAllAsync(customerId: null);

        result.Should().ContainSingle(t => t.Title == "کار اول");
    }

    [Fact]
    public async Task GetByIdAsync_WhenFound_ReturnsTask()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, []);
        _repository.Setup(r => r.GetByIdAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);

        var result = await _sut.GetByIdAsync(task.Id);

        result!.Title.Should().Be("کار");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((TaskItem?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFieldsAndPersists()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, []);
        _repository.Setup(r => r.GetByIdAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        var request = new UpdateTaskRequest("عنوان جدید", "توضیحات جدید", DueAt.AddDays(1), null);

        var result = await _sut.UpdateAsync(task.Id, request);

        result!.Title.Should().Be("عنوان جدید");
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenTaskNotFound_ReturnsNull()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((TaskItem?)null);
        var request = new UpdateTaskRequest("عنوان", string.Empty, DueAt, null);

        var result = await _sut.UpdateAsync(Guid.NewGuid(), request);

        result.Should().BeNull();
    }
}
