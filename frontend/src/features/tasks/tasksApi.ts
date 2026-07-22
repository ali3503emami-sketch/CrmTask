import { httpClient } from '../../shared/api/httpClient'
import type { CreateTaskInput, TaskItem, UpdateTaskInput } from './types'

export const tasksApi = {
  getAll: () => httpClient.get<TaskItem[]>('/api/tasks'),
  create: (input: CreateTaskInput) => httpClient.post<TaskItem>('/api/tasks', input),
  update: (taskId: string, input: UpdateTaskInput) => httpClient.put<TaskItem>(`/api/tasks/${taskId}`, input),
  markAsDone: (taskId: string) => httpClient.post<TaskItem>(`/api/tasks/${taskId}/mark-done`, undefined),
  setChecklistItemValue: (taskId: string, checklistItemId: string, value: string | null) =>
    httpClient.put<TaskItem>(`/api/tasks/${taskId}/checklist-items/${checklistItemId}`, { value }),
}
