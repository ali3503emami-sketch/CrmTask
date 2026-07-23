namespace CrmTask.Domain.ReferenceData;

/// <summary>
/// Which manageable, admin-defined reference list a <see cref="ReferenceListItem"/> belongs to.
/// Each kind backs its own dropdown elsewhere in the app (e.g. a customer's or staff
/// member's "سمت"/position, a customer's "دسته‌بندی"/category or "زمینه فعالیت"/activity field).
/// </summary>
public enum ReferenceListKind
{
    Position,
    CustomerCategory,
    ActivityField,
}
