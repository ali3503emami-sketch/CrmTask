import { httpClient } from '../../shared/api/httpClient'
import type { CreateTaskInput, TaskItem } from './types'

export const tasksApi = {
  getAll: () => httpClient.get<TaskItem[]>('/api/tasks'),
  create: (input: CreateTaskInput) => httpClient.post<TaskItem>('/api/tasks', input),
  markAsDone: (taskId: string) => httpClient.post<TaskItem>(`/api/tasks/${taskId}/mark-done`, undefined),
  setChecklistItemValue: (taskId: string, checklistItemId: string, value: string | null) =>
    httpClient.put<TaskItem>(`/api/tasks/${taskId}/checklist-items/${checklistItemId}`, { value }),
}
