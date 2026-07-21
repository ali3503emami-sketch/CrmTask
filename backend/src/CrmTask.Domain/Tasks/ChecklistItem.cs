namespace CrmTask.Domain.Tasks;

/// <summary>
/// One field of a task's checklist. <see cref="FieldType"/> decides how
/// <see cref="Value"/> is validated and how the frontend renders the field.
/// </summary>
public class ChecklistItem
{
    private static readonly ChecklistFieldType[] ChoiceFieldTypes = [ChecklistFieldType.Dropdown, ChecklistFieldType.ListBox];
    private readonly List<string> _options = [];

    private ChecklistItem()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    public string Label { get; private set; } = null!;

    public ChecklistFieldType FieldType { get; private set; }

    public IReadOnlyList<string> Options => _options;

    public string? Value { get; private set; }

    public static ChecklistItem Create(string label, ChecklistFieldType fieldType, IReadOnlyList<string>? options)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            throw new ArgumentException("Checklist item label is required.", nameof(label));
        }

        var isChoiceField = ChoiceFieldTypes.Contains(fieldType);
        if (isChoiceField && (options is null || options.Count == 0))
        {
            throw new ArgumentException($"{fieldType} checklist items require at least one option.", nameof(options));
        }

        var item = new ChecklistItem
        {
            Id = Guid.NewGuid(),
            Label = label.Trim(),
            FieldType = fieldType,
        };

        if (isChoiceField)
        {
            item._options.AddRange(options!);
        }

        return item;
    }

    public void SetValue(string? value)
    {
        switch (FieldType)
        {
            case ChecklistFieldType.Checkbox when value is not (null or "true" or "false"):
                throw new ArgumentException("Checkbox value must be \"true\" or \"false\".", nameof(value));
            case ChecklistFieldType.Dropdown or ChecklistFieldType.ListBox when value is not null && !_options.Contains(value):
                throw new ArgumentException("Value must be one of the item's options.", nameof(value));
        }

        Value = value;
    }
}
