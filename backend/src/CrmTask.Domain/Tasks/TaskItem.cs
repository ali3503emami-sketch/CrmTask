using CrmTask.Domain.Shared;

namespace CrmTask.Domain.Tasks;

/// <summary>
/// A unit of work — internal (no <see cref="CustomerId"/>) or tied to a customer,
/// assigned to a staff member, with an optional dynamic checklist. Editable while
/// <see cref="TaskItemStatus.Open"/>; locked once <see cref="TaskItemStatus.Done"/>.
/// </summary>
public class TaskItem
{
    private readonly List<ChecklistItem> _checklistItems = [];
    private readonly List<TaskReferral> _referrals = [];

    private TaskItem()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public DateTimeOffset DueAt { get; private set; }

    public string DueAtShamsi { get; private set; } = null!;

    public Guid? CustomerId { get; private set; }

    public Guid AssignedToStaffId { get; private set; }

    /// <summary>
    /// The staff member who registered this task — distinct from <see cref="AssignedToStaffId"/>
    /// (who it's assigned to). Drives the Dashboard's "tasks I registered" tab.
    /// </summary>
    public Guid CreatedByStaffId { get; private set; }

    public TaskItemStatus Status { get; private set; }

    public IReadOnlyList<ChecklistItem> ChecklistItems => _checklistItems;

    public IReadOnlyList<TaskReferral> Referrals => _referrals;

    public static TaskItem Create(
        string title,
        string description,
        DateTimeOffset dueAt,
        Guid? customerId,
        Guid assignedToStaffId,
        Guid createdByStaffId,
        IEnumerable<ChecklistItem> checklistItems)
    {
        if (assignedToStaffId == Guid.Empty)
        {
            throw new ArgumentException("A task must be assigned to a staff member.", nameof(assignedToStaffId));
        }

        if (createdByStaffId == Guid.Empty)
        {
            throw new ArgumentException("A task must record who created it.", nameof(createdByStaffId));
        }

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            AssignedToStaffId = assignedToStaffId,
            CreatedByStaffId = createdByStaffId,
            Status = TaskItemStatus.Open,
        };
        task.Update(title, description, dueAt, customerId, assignedToStaffId);
        task._checklistItems.AddRange(checklistItems);

        return task;
    }

    public void Update(string title, string description, DateTimeOffset dueAt, Guid? customerId, Guid assignedToStaffId)
    {
        EnsureEditable();

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Task title is required.", nameof(title));
        }

        if (assignedToStaffId == Guid.Empty)
        {
            throw new ArgumentException("A task must be assigned to a staff member.", nameof(assignedToStaffId));
        }

        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
        DueAt = dueAt;
        DueAtShamsi = PersianDateConverter.ToShamsi(dueAt);
        CustomerId = customerId;
        AssignedToStaffId = assignedToStaffId;
    }

    public void ReplaceChecklist(IEnumerable<ChecklistItem> checklistItems)
    {
        EnsureEditable();

        _checklistItems.Clear();
        _checklistItems.AddRange(checklistItems);
    }

    /// <summary>
    /// Whether <paramref name="staffId"/> may refer this task onward — the
    /// current assignee, or anyone it has ever been referred to (see <see cref="Refer"/>).
    /// Also determines whether the task shows up in that person's "کارهای جاری" list.
    /// </summary>
    public bool CanRefer(Guid staffId) => staffId == AssignedToStaffId || _referrals.Any(r => r.ReferredToStaffId == staffId);

    /// <summary>
    /// Forwards the task to another staff member with a note — does *not* change
    /// <see cref="AssignedToStaffId"/> (the official assignee never changes via
    /// referral, per product decision). Caller authorization (only the assignee
    /// or a past referral recipient may call this) is enforced by
    /// <c>TaskService.ReferAsync</c>, not here — this method just records the
    /// referral once that's already been checked.
    /// </summary>
    public void Refer(Guid referredByStaffId, Guid referredToStaffId, string note)
    {
        EnsureEditable();

        _referrals.Add(TaskReferral.Create(referredByStaffId, referredToStaffId, note));
    }

    public void MarkAsDone()
    {
        if (Status == TaskItemStatus.Done)
        {
            throw new InvalidOperationException("Task is already marked as done.");
        }

        Status = TaskItemStatus.Done;
    }

    public void Reassign(Guid staffId)
    {
        EnsureEditable();

        if (staffId == Guid.Empty)
        {
            throw new ArgumentException("A task must be assigned to a staff member.", nameof(staffId));
        }

        AssignedToStaffId = staffId;
    }

    public void SetChecklistItemValue(Guid checklistItemId, string? value)
    {
        EnsureEditable();

        var item = _checklistItems.FirstOrDefault(i => i.Id == checklistItemId)
            ?? throw new InvalidOperationException($"Checklist item {checklistItemId} does not belong to this task.");

        item.SetValue(value);
    }

    private void EnsureEditable()
    {
        if (Status == TaskItemStatus.Done)
        {
            throw new InvalidOperationException("A completed task can no longer be edited.");
        }
    }
}
