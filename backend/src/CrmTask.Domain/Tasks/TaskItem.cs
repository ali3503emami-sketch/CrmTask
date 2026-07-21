namespace CrmTask.Domain.Tasks;

/// <summary>
/// A unit of work — internal (no <see cref="CustomerId"/>) or tied to a customer,
/// assigned to a staff member, with an optional dynamic checklist.
/// </summary>
public class TaskItem
{
    private readonly List<ChecklistItem> _checklistItems = [];

    private TaskItem()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public DateTimeOffset DueAt { get; private set; }

    public Guid? CustomerId { get; private set; }

    public Guid AssignedToStaffId { get; private set; }

    public TaskItemStatus Status { get; private set; }

    public IReadOnlyList<ChecklistItem> ChecklistItems => _checklistItems;

    public static TaskItem Create(
        string title,
        string description,
        DateTimeOffset dueAt,
        Guid? customerId,
        Guid assignedToStaffId,
        IEnumerable<ChecklistItem> checklistItems)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Task title is required.", nameof(title));
        }

        if (assignedToStaffId == Guid.Empty)
        {
            throw new ArgumentException("A task must be assigned to a staff member.", nameof(assignedToStaffId));
        }

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Description = description?.Trim() ?? string.Empty,
            DueAt = dueAt,
            CustomerId = customerId,
            AssignedToStaffId = assignedToStaffId,
            Status = TaskItemStatus.Open,
        };
        task._checklistItems.AddRange(checklistItems);

        return task;
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
        if (staffId == Guid.Empty)
        {
            throw new ArgumentException("A task must be assigned to a staff member.", nameof(staffId));
        }

        AssignedToStaffId = staffId;
    }

    public void SetChecklistItemValue(Guid checklistItemId, string? value)
    {
        var item = _checklistItems.FirstOrDefault(i => i.Id == checklistItemId)
            ?? throw new InvalidOperationException($"Checklist item {checklistItemId} does not belong to this task.");

        item.SetValue(value);
    }
}
