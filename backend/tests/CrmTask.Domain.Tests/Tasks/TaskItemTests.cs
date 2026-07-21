using CrmTask.Domain.Tasks;
using FluentAssertions;
using Xunit;

namespace CrmTask.Domain.Tests.Tasks;

public class TaskItemTests
{
    private static readonly DateTimeOffset DueAt = new(2026, 8, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid StaffId = Guid.NewGuid();
    private static readonly Guid CustomerId = Guid.NewGuid();

    [Fact]
    public void Create_InternalTask_WithoutCustomer_Succeeds()
    {
        var task = TaskItem.Create("بررسی سرور", "بررسی وضعیت سرور پشتیبان", DueAt, customerId: null, assignedToStaffId: StaffId, []);

        task.Id.Should().NotBeEmpty();
        task.Title.Should().Be("بررسی سرور");
        task.CustomerId.Should().BeNull();
        task.AssignedToStaffId.Should().Be(StaffId);
        task.Status.Should().Be(TaskItemStatus.Open);
        task.DueAt.Should().Be(DueAt);
    }

    [Fact]
    public void Create_CustomerTask_WithCustomerId_Succeeds()
    {
        var task = TaskItem.Create("پیگیری قرارداد", string.Empty, DueAt, CustomerId, StaffId, []);

        task.CustomerId.Should().Be(CustomerId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithMissingTitle_Throws(string? title)
    {
        var act = () => TaskItem.Create(title!, string.Empty, DueAt, null, StaffId, []);

        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Fact]
    public void Create_WithChecklistItems_ExposesThem()
    {
        var checklist = new[]
        {
            ChecklistItem.Create("چک شد؟", ChecklistFieldType.Checkbox, null),
            ChecklistItem.Create("توضیحات", ChecklistFieldType.TextBox, null),
        };

        var task = TaskItem.Create("کار با چک‌لیست", string.Empty, DueAt, null, StaffId, checklist);

        task.ChecklistItems.Should().HaveCount(2);
    }

    [Fact]
    public void MarkAsDone_TransitionsStatusToDone()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, []);

        task.MarkAsDone();

        task.Status.Should().Be(TaskItemStatus.Done);
    }

    [Fact]
    public void MarkAsDone_WhenAlreadyDone_Throws()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, []);
        task.MarkAsDone();

        var act = task.MarkAsDone;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Reassign_ChangesAssignedStaff()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, []);
        var newStaffId = Guid.NewGuid();

        task.Reassign(newStaffId);

        task.AssignedToStaffId.Should().Be(newStaffId);
    }

    [Fact]
    public void Reassign_WithEmptyId_Throws()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, []);

        var act = () => task.Reassign(Guid.Empty);

        act.Should().Throw<ArgumentException>().WithParameterName("staffId");
    }

    [Fact]
    public void SetChecklistItemValue_UpdatesTheMatchingItem()
    {
        var checklistItem = ChecklistItem.Create("چک شد؟", ChecklistFieldType.Checkbox, null);
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, [checklistItem]);

        task.SetChecklistItemValue(checklistItem.Id, "true");

        task.ChecklistItems.Single().Value.Should().Be("true");
    }

    [Fact]
    public void SetChecklistItemValue_WithUnknownItemId_Throws()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, []);

        var act = () => task.SetChecklistItemValue(Guid.NewGuid(), "true");

        act.Should().Throw<InvalidOperationException>();
    }
}
