import { useMutation, useQueryClient } from '@tanstack/react-query'
import { tasksApi } from './tasksApi'

export function useMarkTaskDone() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (taskId: string) => tasksApi.markAsDone(taskId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks'] })
    },
  })
}
