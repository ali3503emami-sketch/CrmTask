import { useQuery } from '@tanstack/react-query'
import { tasksApi } from './tasksApi'

export function useTasks() {
  return useQuery({
    queryKey: ['tasks'],
    queryFn: tasksApi.getAll,
  })
}
