import { useMutation, useQueryClient } from '@tanstack/react-query'
import { tasksApi } from './tasksApi'

export function useSetChecklistItemValue() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ taskId, checklistItemId, value }: { taskId: string; checklistItemId: string; value: string | null }) =>
      tasksApi.setChecklistItemValue(taskId, checklistItemId, value),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks'] })
    },
  })
}
