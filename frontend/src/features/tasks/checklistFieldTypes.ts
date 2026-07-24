import type { ChecklistFieldType, ChecklistItem } from './types'

export const fieldTypeOptions: { value: ChecklistFieldType; label: string }[] = [
  { value: 'Checkbox', label: 'چک‌باکس' },
  { value: 'TextBox', label: 'متن تک‌خط' },
  { value: 'MultilineText', label: 'متن چندخط' },
  { value: 'Dropdown', label: 'کشو' },
]

export const choiceFieldTypes: ChecklistFieldType[] = ['Dropdown']

/** Each line of the options textarea is one dropdown item — not comma-separated. */
export function parseDropdownOptions(text: string): string[] {
  return text
    .split('\n')
    .map((o) => o.trim())
    .filter(Boolean)
}

export interface ChecklistFieldFormValue {
  label: string
  fieldType: ChecklistFieldType
  options?: string
}

export function checklistItemsToFormValues(items: ChecklistItem[]): ChecklistFieldFormValue[] {
  return items.map((i) => ({ label: i.label, fieldType: i.fieldType, options: i.options.join('\n') }))
}
