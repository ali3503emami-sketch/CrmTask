export type ChecklistFieldType = 'Checkbox' | 'Dropdown' | 'ListBox' | 'TextBox'
export type TaskItemStatus = 'Open' | 'Done'

export interface ChecklistFieldDefinition {
  label: string
  fieldType: ChecklistFieldType
  options: string[] | null
}

export interface ChecklistItem {
  id: string
  label: string
  fieldType: ChecklistFieldType
  options: string[]
  value: string | null
}

export interface TaskItem {
  id: string
  title: string
  description: string
  dueAt: string
  customerId: string | null
  assignedToStaffId: string
  status: TaskItemStatus
  checklistItems: ChecklistItem[]
}

export interface CreateTaskInput {
  title: string
  description: string
  dueAt: string
  customerId: string | null
  assignedToStaffId: string
  checklistFields: ChecklistFieldDefinition[]
}
