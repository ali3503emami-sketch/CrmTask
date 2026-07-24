namespace CrmTask.Domain.Tasks;

/// <summary>
/// The kind of input a checklist item collects — matches the scenario's
/// "چک‌باکس، متن تک‌خط، متن چندخط، کشو" requirement.
/// </summary>
public enum ChecklistFieldType
{
    Checkbox,
    Dropdown,
    TextBox,
    MultilineText,
}
