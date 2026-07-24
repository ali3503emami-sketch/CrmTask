import { useMutation, useQueryClient } from '@tanstack/react-query'
import { tasksApi } from './tasksApi'
import type { ReferTaskInput } from './types'

export function useReferTask() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ taskId, input }: { taskId: string; input: ReferTaskInput }) => tasksApi.refer(taskId, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks'] })
    },
  })
}
