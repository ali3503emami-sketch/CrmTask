import { useMutation, useQueryClient } from '@tanstack/react-query'
import { tasksApi } from './tasksApi'
import type { UpdateTaskInput } from './types'

export function useUpdateTask() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ taskId, input }: { taskId: string; input: UpdateTaskInput }) => tasksApi.update(taskId, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks'] })
    },
  })
}
