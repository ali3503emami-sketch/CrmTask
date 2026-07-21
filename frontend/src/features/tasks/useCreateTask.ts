import { useMutation, useQueryClient } from '@tanstack/react-query'
import { tasksApi } from './tasksApi'

export function useCreateTask() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: tasksApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks'] })
    },
  })
}
