namespace CrmTask.Domain.ReferenceData;

/// <summary>
/// A single named entry in one of the app's manageable reference lists (see
/// <see cref="ReferenceListKind"/>) — deliberately just "Id + Kind + Title": these
/// are simple, admin-curated vocabularies (سمت‌ها، دسته‌بندی مشتریان، زمینه فعالیت),
/// not entities with their own further behavior. One table/entity shared across all
/// three kinds rather than three near-identical entities, per the project's
/// no-redundant-code rule — they really are 100% identical in shape.
/// </summary>
public class ReferenceListItem
{
    private ReferenceListItem()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    public ReferenceListKind Kind { get; private set; }

    public string Title { get; private set; } = null!;

    public static ReferenceListItem Create(ReferenceListKind kind, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        return new ReferenceListItem
        {
            Id = Guid.NewGuid(),
            Kind = kind,
            Title = title.Trim(),
        };
    }
}
