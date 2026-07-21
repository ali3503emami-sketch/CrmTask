namespace CrmTask.Domain.Tasks;

/// <summary>
/// The kind of input a checklist item collects — matches the scenario's
/// "چک‌باکس، کشو، لیست‌باکس، تکست‌باکس" requirement.
/// </summary>
public enum ChecklistFieldType
{
    Checkbox,
    Dropdown,
    ListBox,
    TextBox,
}
