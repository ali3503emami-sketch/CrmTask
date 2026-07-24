using CrmTask.Domain.Shared;

namespace CrmTask.Domain.Tasks;

/// <summary>
/// A record of one "ارجاع" (referral) — forwarding a task to another staff
/// member with a note, without changing <see cref="TaskItem.AssignedToStaffId"/>.
/// Kept as full history (not just the latest) since anyone a task has ever been
/// referred to keeps the ability to refer it onward — see <see cref="TaskItem.CanRefer"/>.
/// </summary>
public class TaskReferral
{
    private TaskReferral()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    public Guid ReferredByStaffId { get; private set; }

    public Guid ReferredToStaffId { get; private set; }

    public string Note { get; private set; } = null!;

    public DateTimeOffset ReferredAt { get; private set; }

    public string ReferredAtShamsi { get; private set; } = null!;

    public static TaskReferral Create(Guid referredByStaffId, Guid referredToStaffId, string note)
    {
        if (referredToStaffId == Guid.Empty)
        {
            throw new ArgumentException("A referral must name who it's referred to.", nameof(referredToStaffId));
        }

        if (string.IsNullOrWhiteSpace(note))
        {
            throw new ArgumentException("Referral note is required.", nameof(note));
        }

        var referredAt = DateTimeOffset.UtcNow;
        return new TaskReferral
        {
            Id = Guid.NewGuid(),
            ReferredByStaffId = referredByStaffId,
            ReferredToStaffId = referredToStaffId,
            Note = note.Trim(),
            ReferredAt = referredAt,
            ReferredAtShamsi = PersianDateConverter.ToShamsi(referredAt),
        };
    }
}
