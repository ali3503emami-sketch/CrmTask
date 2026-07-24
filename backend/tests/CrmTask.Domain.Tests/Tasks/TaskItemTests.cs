using CrmTask.Domain.Tasks;
using FluentAssertions;
using Xunit;

namespace CrmTask.Domain.Tests.Tasks;

public class TaskItemTests
{
    private static readonly DateTimeOffset DueAt = new(2026, 8, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid StaffId = Guid.NewGuid();
    private static readonly Guid CreatedByStaffId = Guid.NewGuid();
    private static readonly Guid CustomerId = Guid.NewGuid();

    [Fact]
    public void Create_InternalTask_WithoutCustomer_Succeeds()
    {
        var task = TaskItem.Create("بررسی سرور", "بررسی وضعیت سرور پشتیبان", DueAt, customerId: null, assignedToStaffId: StaffId, createdByStaffId: CreatedByStaffId, []);

        task.Id.Should().NotBeEmpty();
        task.Title.Should().Be("بررسی سرور");
        task.CustomerId.Should().BeNull();
        task.AssignedToStaffId.Should().Be(StaffId);
        task.CreatedByStaffId.Should().Be(CreatedByStaffId);
        task.Status.Should().Be(TaskItemStatus.Open);
        task.DueAt.Should().Be(DueAt);
    }

    [Fact]
    public void Create_WithEmptyCreatedByStaffId_Throws()
    {
        var act = () => TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, Guid.Empty, []);

        act.Should().Throw<ArgumentException>().WithParameterName("createdByStaffId");
    }

    [Fact]
    public void Create_CustomerTask_WithCustomerId_Succeeds()
    {
        var task = TaskItem.Create("پیگیری قرارداد", string.Empty, DueAt, CustomerId, StaffId, CreatedByStaffId, []);

        task.CustomerId.Should().Be(CustomerId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithMissingTitle_Throws(string? title)
    {
        var act = () => TaskItem.Create(title!, string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);

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

        var task = TaskItem.Create("کار با چک‌لیست", string.Empty, DueAt, null, StaffId, CreatedByStaffId, checklist);

        task.ChecklistItems.Should().HaveCount(2);
    }

    [Fact]
    public void MarkAsDone_TransitionsStatusToDone()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);

        task.MarkAsDone();

        task.Status.Should().Be(TaskItemStatus.Done);
    }

    [Fact]
    public void MarkAsDone_WhenAlreadyDone_Throws()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);
        task.MarkAsDone();

        var act = task.MarkAsDone;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Reassign_ChangesAssignedStaff()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);
        var newStaffId = Guid.NewGuid();

        task.Reassign(newStaffId);

        task.AssignedToStaffId.Should().Be(newStaffId);
    }

    [Fact]
    public void Reassign_WithEmptyId_Throws()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);

        var act = () => task.Reassign(Guid.Empty);

        act.Should().Throw<ArgumentException>().WithParameterName("staffId");
    }

    [Fact]
    public void SetChecklistItemValue_UpdatesTheMatchingItem()
    {
        var checklistItem = ChecklistItem.Create("چک شد؟", ChecklistFieldType.Checkbox, null);
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, [checklistItem]);

        task.SetChecklistItemValue(checklistItem.Id, "true");

        task.ChecklistItems.Single().Value.Should().Be("true");
    }

    [Fact]
    public void SetChecklistItemValue_WithUnknownItemId_Throws()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);

        var act = () => task.SetChecklistItemValue(Guid.NewGuid(), "true");

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Create_SetsDueAtShamsi()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);

        task.DueAtShamsi.Should().Be(CrmTask.Domain.Shared.PersianDateConverter.ToShamsi(DueAt));
    }

    [Fact]
    public void Update_ChangesTitleDescriptionDueAtCustomerAndAssignee()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);
        var newDueAt = DueAt.AddDays(1);
        var newAssigneeId = Guid.NewGuid();

        task.Update("عنوان جدید", "توضیحات جدید", newDueAt, CustomerId, newAssigneeId);

        task.Title.Should().Be("عنوان جدید");
        task.Description.Should().Be("توضیحات جدید");
        task.DueAt.Should().Be(newDueAt);
        task.DueAtShamsi.Should().Be(CrmTask.Domain.Shared.PersianDateConverter.ToShamsi(newDueAt));
        task.CustomerId.Should().Be(CustomerId);
        task.AssignedToStaffId.Should().Be(newAssigneeId);
    }

    [Fact]
    public void Update_WithEmptyAssignedToStaffId_Throws()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);

        var act = () => task.Update("عنوان جدید", string.Empty, DueAt, null, Guid.Empty);

        act.Should().Throw<ArgumentException>().WithParameterName("assignedToStaffId");
    }

    [Fact]
    public void Update_WhenTaskIsDone_Throws()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);
        task.MarkAsDone();

        var act = () => task.Update("عنوان جدید", string.Empty, DueAt, null, StaffId);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ReplaceChecklist_SwapsInNewItems()
    {
        var original = ChecklistItem.Create("قدیمی", ChecklistFieldType.TextBox, null);
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, [original]);
        var replacement = ChecklistItem.Create("جدید", ChecklistFieldType.Checkbox, null);

        task.ReplaceChecklist([replacement]);

        task.ChecklistItems.Should().ContainSingle(i => i.Label == "جدید");
    }

    [Fact]
    public void ReplaceChecklist_WhenTaskIsDone_Throws()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);
        task.MarkAsDone();

        var act = () => task.ReplaceChecklist([]);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CanRefer_ForAssignee_ReturnsTrue()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);

        task.CanRefer(StaffId).Should().BeTrue();
    }

    [Fact]
    public void CanRefer_ForUnrelatedStaff_ReturnsFalse()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);

        task.CanRefer(Guid.NewGuid()).Should().BeFalse();
    }

    [Fact]
    public void Refer_AddsReferralAndDoesNotChangeAssignee()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);
        var referredToId = Guid.NewGuid();

        task.Refer(StaffId, referredToId, "لطفاً پیگیری کنید");

        task.AssignedToStaffId.Should().Be(StaffId);
        task.Referrals.Should().ContainSingle(r =>
            r.ReferredByStaffId == StaffId && r.ReferredToStaffId == referredToId && r.Note == "لطفاً پیگیری کنید");
    }

    [Fact]
    public void CanRefer_ForPastReferralRecipient_ReturnsTrue()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);
        var referredToId = Guid.NewGuid();
        task.Refer(StaffId, referredToId, "لطفاً پیگیری کنید");

        task.CanRefer(referredToId).Should().BeTrue();
    }

    [Fact]
    public void Refer_WhenTaskIsDone_Throws()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);
        task.MarkAsDone();

        var act = () => task.Refer(StaffId, Guid.NewGuid(), "یادداشت");

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Reassign_WhenTaskIsDone_Throws()
    {
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, []);
        task.MarkAsDone();

        var act = () => task.Reassign(Guid.NewGuid());

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void SetChecklistItemValue_WhenTaskIsDone_Throws()
    {
        var checklistItem = ChecklistItem.Create("چک شد؟", ChecklistFieldType.Checkbox, null);
        var task = TaskItem.Create("کار", string.Empty, DueAt, null, StaffId, CreatedByStaffId, [checklistItem]);
        task.MarkAsDone();

        var act = () => task.SetChecklistItemValue(checklistItem.Id, "true");

        act.Should().Throw<InvalidOperationException>();
    }
}
