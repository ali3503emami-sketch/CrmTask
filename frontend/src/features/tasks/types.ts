export type ChecklistFieldType = 'Checkbox' | 'Dropdown' | 'TextBox' | 'MultilineText'
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

export interface TaskReferral {
  id: string
  referredByStaffId: string
  referredToStaffId: string
  note: string
  referredAtShamsi: string
}

export interface TaskItem {
  id: string
  title: string
  description: string
  dueAt: string
  dueAtShamsi: string
  customerId: string | null
  assignedToStaffId: string
  createdByStaffId: string
  status: TaskItemStatus
  checklistItems: ChecklistItem[]
  referrals: TaskReferral[]
}

export interface CreateTaskInput {
  title: string
  description: string
  dueAt: string
  customerId: string | null
  assignedToStaffId: string
  createdByStaffId: string
  checklistFields: ChecklistFieldDefinition[]
}

export interface UpdateTaskInput {
  title: string
  description: string
  dueAt: string
  customerId: string | null
  assignedToStaffId: string
  requestedByStaffId: string
  checklistFields: ChecklistFieldDefinition[]
}

export interface ReferTaskInput {
  referredByStaffId: string
  referredToStaffId: string
  note: string
}
